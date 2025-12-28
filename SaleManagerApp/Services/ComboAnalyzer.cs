using System;
using System.Collections.Generic;
using System.Linq;
using SaleManagerApp.Models;

namespace SaleManagerApp.Services
{
    /// <summary>
    /// Phân tích giỏ hàng (Market Basket Analysis) để tìm combo
    /// </summary>
    public class ComboAnalyzer
    {
        private const double MIN_SUPPORT = 0.05;      // 5% đơn hàng phải có combo này
        private const double MIN_CONFIDENCE = 0.3;    // 30% khả năng mua kèm
        private const int MIN_COMBO_SIZE = 2;         // Combo tối thiểu 2 món
        private const int MAX_COMBO_SIZE = 4;         // Combo tối đa 4 món

        /// <summary>
        /// Phân tích và đề xuất combo từ dữ liệu đơn hàng
        /// </summary>
        public List<ComboRecommendation> AnalyzeAndRecommend(List<OrderItem> orderItems, Dictionary<string, MenuItem> menuItemDict)
        {
            // Bước 1: Nhóm các món theo đơn hàng
            var transactions = GroupByTransaction(orderItems);

            if (transactions.Count < 10)
            {
                Console.WriteLine("⚠️ Không đủ dữ liệu để phân tích combo (cần ít nhất 10 đơn)");
                return new List<ComboRecommendation>();
            }

            Console.WriteLine($"📊 Phân tích {transactions.Count} đơn hàng...");

            // Bước 2: Tìm các itemset phổ biến (Apriori Algorithm)
            var frequentItemSets = FindFrequentItemSets(transactions);

            Console.WriteLine($"✅ Tìm thấy {frequentItemSets.Count} combo phổ biến");

            // Bước 3: Tạo quy tắc kết hợp
            var rules = GenerateAssociationRules(frequentItemSets, transactions);

            Console.WriteLine($"✅ Tạo được {rules.Count} quy tắc kết hợp");

            // Bước 4: Chuyển đổi thành combo recommendation
            var recommendations = ConvertToRecommendations(rules, menuItemDict);

            // Bước 5: Sắp xếp theo độ ưu tiên
            return recommendations
                .OrderByDescending(r => r.Confidence)
                .ThenByDescending(r => r.Support)
                .Take(10)
                .ToList();
        }

        /// <summary>
        /// BƯỚC 1: Nhóm món theo đơn hàng
        /// Input: List OrderItem (mỗi dòng = 1 món trong 1 đơn)
        /// Output: Dictionary (OrderId -> List MenuItemId)
        /// </summary>
        private Dictionary<string, HashSet<string>> GroupByTransaction(List<OrderItem> orderItems)
        {
            var transactions = new Dictionary<string, HashSet<string>>();

            foreach (var item in orderItems)
            {
                if (!transactions.ContainsKey(item.OrderId))
                {
                    transactions[item.OrderId] = new HashSet<string>();
                }
                transactions[item.OrderId].Add(item.MenuItemId);
            }

            return transactions;
        }

        /// <summary>
        /// BƯỚC 2: Tìm itemset phổ biến (Apriori Algorithm đơn giản hóa)
        /// Ý tưởng: Đếm xem combo nào xuất hiện đủ nhiều lần
        /// </summary>
        private Dictionary<ItemSet, int> FindFrequentItemSets(Dictionary<string, HashSet<string>> transactions)
        {
            var frequentSets = new Dictionary<ItemSet, int>();
            int totalTransactions = transactions.Count;

            // Sinh tất cả các combo có thể (2 món, 3 món, 4 món)
            var allItems = transactions.SelectMany(t => t.Value).Distinct().ToList();

            // Tìm combo 2 món
            for (int i = 0; i < allItems.Count; i++)
            {
                for (int j = i + 1; j < allItems.Count; j++)
                {
                    var itemSet = new ItemSet(new HashSet<string> { allItems[i], allItems[j] });
                    int support = CountSupport(itemSet, transactions);

                    double supportRatio = (double)support / totalTransactions;
                    if (supportRatio >= MIN_SUPPORT)
                    {
                        frequentSets[itemSet] = support;
                    }
                }
            }

            // Tìm combo 3 món (từ các combo 2 món phổ biến)
            var twoItemSets = frequentSets.Keys.Where(k => k.Items.Count == 2).ToList();
            for (int i = 0; i < twoItemSets.Count; i++)
            {
                for (int j = i + 1; j < twoItemSets.Count; j++)
                {
                    var union = new HashSet<string>(twoItemSets[i].Items);
                    union.UnionWith(twoItemSets[j].Items);

                    if (union.Count == 3)
                    {
                        var itemSet = new ItemSet(union);
                        int support = CountSupport(itemSet, transactions);

                        double supportRatio = (double)support / totalTransactions;
                        if (supportRatio >= MIN_SUPPORT)
                        {
                            frequentSets[itemSet] = support;
                        }
                    }
                }
            }

            return frequentSets;
        }

        /// <summary>
        /// Đếm số đơn hàng có chứa itemset này
        /// </summary>
        private int CountSupport(ItemSet itemSet, Dictionary<string, HashSet<string>> transactions)
        {
            int count = 0;
            foreach (var transaction in transactions.Values)
            {
                if (itemSet.Items.IsSubsetOf(transaction))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// BƯỚC 3: Tạo quy tắc kết hợp từ itemset phổ biến
        /// Ví dụ: {Gà, Pepsi} -> tạo quy tắc "Gà → Pepsi" và "Pepsi → Gà"
        /// </summary>
        private List<AssociationRule> GenerateAssociationRules(
            Dictionary<ItemSet, int> frequentItemSets,
            Dictionary<string, HashSet<string>> transactions)
        {
            var rules = new List<AssociationRule>();
            int totalTransactions = transactions.Count;

            foreach (var itemSet in frequentItemSets.Keys.Where(k => k.Items.Count >= 2))
            {
                // Thử tất cả cách chia itemset thành 2 phần
                var items = itemSet.Items.ToList();

                // Với mỗi item, thử làm consequent
                foreach (var consequent in items)
                {
                    var antecedent = items.Where(i => i != consequent).ToHashSet();

                    // Tính Confidence: P(B|A) = Support(A ∪ B) / Support(A)
                    int supportAB = frequentItemSets[itemSet];
                    int supportA = CountSupport(new ItemSet(antecedent), transactions);

                    if (supportA == 0) continue;

                    double confidence = (double)supportAB / supportA;
                    double support = (double)supportAB / totalTransactions;

                    if (confidence >= MIN_CONFIDENCE)
                    {
                        rules.Add(new AssociationRule
                        {
                            Antecedent = antecedent,
                            Consequent = new HashSet<string> { consequent },
                            Confidence = confidence,
                            Support = support
                        });
                    }
                }
            }

            return rules;
        }

        /// <summary>
        /// BƯỚC 4: Chuyển rules thành combo recommendation với thông tin đầy đủ
        /// </summary>
        private List<ComboRecommendation> ConvertToRecommendations(
            List<AssociationRule> rules,
            Dictionary<string, MenuItem> menuItemDict)
        {
            var recommendations = new List<ComboRecommendation>();

            // Nhóm các rules để tạo combo hoàn chỉnh
            var groupedRules = rules
                .GroupBy(r => string.Join(",", r.Antecedent.Union(r.Consequent).OrderBy(x => x)))
                .ToList();

            foreach (var group in groupedRules)
            {
                var rule = group.First();
                var allItems = rule.Antecedent.Union(rule.Consequent).ToList();

                // Lấy thông tin món từ dictionary
                var itemDetails = allItems
                    .Where(id => menuItemDict.ContainsKey(id))
                    .Select(id => menuItemDict[id])
                    .ToList();

                if (itemDetails.Count == 0) continue;

                var combo = new ComboRecommendation
                {
                    MenuItemIds = allItems,
                    MenuItemNames = itemDetails.Select(i => i.menuItemName).ToList(),
                    ImageUrls = itemDetails.Select(i => i.imageUrl ?? "").ToList(),
                    OriginalPrice = itemDetails.Sum(i => i.unitPrice),
                    Support = rule.Support,
                    Confidence = rule.Confidence
                };

                recommendations.Add(combo);
            }

            return recommendations;
        }

        /// <summary>
        /// Tính giá combo sau giảm dựa trên phân bố giá đơn hàng
        /// </summary>
        public decimal CalculateUniformComboPriceFromAverageCost(
     List<ComboRecommendation> topCombos,
     List<decimal> orderValues)
        {
            if (topCombos == null || topCombos.Count == 0)
            {
                Console.WriteLine("⚠️ Không có combo để tính giá, dùng giá mặc định 50.000 VNĐ");
                return 50000m;
            }

            // Tính trung bình giá gốc của các combo
            decimal averageOriginalPrice = topCombos.Average(c => c.OriginalPrice);

            Console.WriteLine($"📊 Phân tích giá các combo:");
            foreach (var combo in topCombos)
            {
                Console.WriteLine($"   - {string.Join(" + ", combo.MenuItemNames)}: {combo.OriginalPrice:N0} VNĐ");
            }
            Console.WriteLine($"   → Trung bình giá gốc: {averageOriginalPrice:N0} VNĐ");

            // Tính median giá đơn hàng để tham khảo
            decimal medianOrderValue = 0;
            if (orderValues != null && orderValues.Count > 0)
            {
                orderValues.Sort();
                medianOrderValue = orderValues[orderValues.Count / 2];
                Console.WriteLine($"   → Median giá đơn hàng: {medianOrderValue:N0} VNĐ");
            }

            // Xác định % giảm giá hợp lý
            decimal discountPercent;

            if (averageOriginalPrice > medianOrderValue && medianOrderValue > 0)
            {
                // Nếu giá combo cao hơn median đơn hàng → giảm nhiều hơn để hấp dẫn
                discountPercent = 0.25m; // Giảm 25%
                Console.WriteLine($"   → Giá combo cao hơn median → Giảm 25%");
            }
            else
            {
                // Giá combo hợp lý → giảm vừa phải
                discountPercent = 0.20m; // Giảm 20%
                Console.WriteLine($"   → Giá combo hợp lý → Giảm 20%");
            }

            // Tính giá sau giảm
            decimal discountedPrice = averageOriginalPrice * (1 - discountPercent);

            // Làm tròn đến bội số của 1.000 VNĐ
            decimal roundedPrice = Math.Round(discountedPrice / 1000) * 1000;

            // Đảm bảo giá tối thiểu
            decimal minPrice = topCombos.Min(c => c.OriginalPrice) * 0.7m; // Ít nhất bằng 70% giá rẻ nhất
            decimal finalPrice = Math.Max(roundedPrice, Math.Round(minPrice / 1000) * 1000);

            // Đảm bảo giá tối thiểu tuyệt đối
            finalPrice = Math.Max(finalPrice, 30000m);

            Console.WriteLine($"");
            Console.WriteLine($"💰 GIÁ ĐỒNG NHẤT: {finalPrice:N0} VNĐ (giảm {discountPercent * 100:N0}% từ TB)");
            Console.WriteLine($"");

            // Tính lãi/lỗ cho từng combo
            Console.WriteLine($"📈 Phân tích lãi/lỗ:");
            foreach (var combo in topCombos)
            {
                decimal diff = finalPrice - combo.OriginalPrice;
                decimal diffPercent = (diff / combo.OriginalPrice) * 100;
                string status = diff >= 0 ? "✅ Lãi" : "⚠️ Lỗ";
                Console.WriteLine($"   {status} {Math.Abs(diffPercent):N1}% so với {string.Join(" + ", combo.MenuItemNames.Take(2))}");
            }
            Console.WriteLine($"");

            return finalPrice;
        }

        /// <summary>
        /// Lọc chỉ lấy combo có số lượng món BẰNG NHAU
        /// Ưu tiên combo 3 món để đồng giá hợp lý
        /// </summary>
        public List<ComboRecommendation> FilterUniformSizeCombo(List<ComboRecommendation> recommendations)
        {
            if (recommendations.Count == 0)
                return recommendations;

            // Đếm số món trong mỗi combo
            var comboBySizes = recommendations
                .GroupBy(r => r.MenuItemIds.Count)
                .OrderByDescending(g => g.Key) // Ưu tiên combo nhiều món hơn
                .ToList();

            // Lấy kích thước combo phổ biến nhất và đủ lớn (≥3 món)
            var targetSize = comboBySizes
                .Where(g => g.Key >= 3) // Chỉ lấy combo ≥3 món
                .FirstOrDefault();

            if (targetSize == null)
            {
                // Nếu không có combo 3 món, lấy size lớn nhất có thể
                targetSize = comboBySizes.First();
                Console.WriteLine($"⚠️ Không có combo 3 món, lấy combo {targetSize.Key} món");
            }
            else
            {
                Console.WriteLine($"✅ Lọc chỉ giữ combo {targetSize.Key} món");
            }

            var filtered = targetSize.ToList();

            if (filtered.Count < 3)
            {
                Console.WriteLine($"⚠️ Chỉ có {filtered.Count} combo {targetSize.Key} món, thêm combo size khác để đủ 3");

                // Bổ sung thêm combo từ size gần nhất
                var otherSizes = comboBySizes
                    .Where(g => g.Key != targetSize.Key)
                    .SelectMany(g => g)
                    .Take(3 - filtered.Count);

                filtered.AddRange(otherSizes);
            }

            return filtered;
        }
    }
}

