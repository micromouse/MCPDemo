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
        /// 销售额
        /// </summary>
        public decimal TotalSales { get; init; }
    }
}
