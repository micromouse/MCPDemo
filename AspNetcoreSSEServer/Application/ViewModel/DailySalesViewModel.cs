namespace AspNetcoreSSEServer.Application.ViewModel {
    /// <summary>
    /// DailySalesViewModel - ViewModel for daily sales data
    /// </summary>
    public class DailySalesViewModel {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; init; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string WaresName { get; init; } = null!;
        /// <summary>
        /// 销售额
        /// </summary>
        public decimal TotalSales { get; init; }
    }
}
