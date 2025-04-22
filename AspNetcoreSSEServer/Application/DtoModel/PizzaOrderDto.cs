namespace AspNetcoreSSEServer.Application.DtoModel {
    /// <summary>
    /// PizzaOrderSessionDto - Data Transfer Object for pizza order session
    /// </summary>
    public class PizzaOrderSessionDto {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; init; } = null!;
        /// <summary>
        /// Pizza类型
        /// </summary>
        public string PizzaType { get; set; } = null!;
        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PaymentMethod { get; set; } = null!;
        /// <summary>
        /// 总价
        /// </summary>
        public decimal? TotalPrice { get; set; }
        /// <summary>
        /// 是否已支付
        /// </summary>
        public bool IsPaid { get; set; }
    }

    /// <summary>
    /// PizzaOrderResultDto - Data Transfer Object for pizza order result
    /// </summary>
    public class PizzaOrderResultDto {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; init; } = null!;
        /// <summary>
        /// Pizza类型
        /// </summary>
        public string PizzaType { get; init; } = null!;
        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; init; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; init; } = null!;
    }

    /// <summary>
    /// PizzaPaymentResultDto - Data Transfer Object for pizza payment result
    /// </summary>
    public class PizzaPaymentResultDto {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; init; } = null!;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; init; } = null!;
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PaymentMethod { get; init; } = null!;
        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal? PaidAmount { get; init; }
    }
}