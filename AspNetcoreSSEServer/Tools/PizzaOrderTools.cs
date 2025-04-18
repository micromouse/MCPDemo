using AspNetcoreSSEServer.Application.DtoModel;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace AspNetcoreSSEServer.Tools {
    /// <summary>
    /// PizzaOrderTools - A tool for managing pizza orders
    /// </summary>
    [McpServerToolType, Description("披萨订单处理工具")]
    public class PizzaOrderTools {
        private static readonly Dictionary<string, PizzaOrderSessionDto> _orders = new();

        /// <summary>
        /// 开始一个披萨订单
        /// </summary>
        /// <param name="pizzaType">披萨类型</param>
        /// <param name="quantity">数量</param>
        /// <returns>结果</returns>
        [McpServerTool, Description("开始一个披萨订单（必须提供披萨类型和订购数量，不允许假设数量为1）")]
        public PizzaOrderResultDto StartPizzaOrder(
            [Description("披萨类型，例如：奶酪、夏威夷")] string pizzaType,
            [Description("订购数量,用户必须明确输入订购数量")] int? quantity
        ) {
            var orderId = Ulid.NewUlid().ToString();

            //必须提供订购数量
            if (quantity == null || quantity <= 0) {
                throw new ArgumentNullException(nameof(quantity), "订购数量必须由用户明确提供");
            }

            //添加订单Session
            _orders.Add(orderId, new() {
                OrderId = orderId,
                PizzaType = pizzaType,
                Quantity = quantity
            });

            return new PizzaOrderResultDto {
                OrderId = orderId,
                PizzaType = pizzaType,
                Quantity = quantity,
                Status = "已创建"
            };
        }

        /// <summary>
        /// 支付披萨订单
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="paymentMethod">支付方式</param>
        /// <param name="totalPrice">支付金额</param>
        /// <returns>支付结果</returns>
        [McpServerTool, Description("支付披萨订单")]
        public PizzaPaymentResultDto PayForPizzaOrder(
            [Description("订单Id")] string orderId,
            [Description("支付方式，如：支付宝、微信、信用卡")] string paymentMethod,
            [Description("为订单支付，必须提供支付金额")] decimal? totalPrice
        ) {
            if (!_orders.TryGetValue(orderId, out var session)) {
                throw new ArgumentException($"订单Id {orderId} 不存在");
            }

            //支付
            session.PaymentMethod = paymentMethod;
            session.TotalPrice = totalPrice;
            session.IsPaid = true;

            //结果
            return new PizzaPaymentResultDto {
                OrderId = orderId,
                Message = (totalPrice ?? 0) <= 0 ? "支付金额为0元，请确认订单已正确结算" : $"订单 {orderId} 已支付成功，支付方式：{paymentMethod}，金额：{totalPrice} 元",
                PaymentMethod = paymentMethod,
                PaidAmount = totalPrice
            };
        }
    }
}
