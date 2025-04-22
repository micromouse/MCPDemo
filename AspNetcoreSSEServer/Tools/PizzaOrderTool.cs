using AspNetcoreSSEServer.Application.DtoModel;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AspNetcoreSSEServer.Tools {
    /// <summary>
    /// PizzaOrderTools - A tool for managing pizza orders
    /// </summary>
    [McpServerToolType, Description("披萨订单处理工具")]
    public class PizzaOrderTool {
        private static readonly Dictionary<string, PizzaOrderSessionDto> _orders = [];

        /// <summary>
        /// 获得指定订单Id的披萨订单信息
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <returns>披萨订单信息</returns>
        [McpServerTool, Description("获取指定订单Id的披萨订单信息")]
        public PizzaOrderSessionDto GetPizzaOrderSession([Description("披萨订单Id"), Required] string orderId) {
            if (_orders.TryGetValue(orderId, out var session)) {
                return session;
            } else {
                throw new ArgumentException($"订单Id {orderId} 不存在");
            }
        }

        /// <summary>
        /// 更新披萨订单信息
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="session">新披萨订单信息</param>
        /// <returns>新披萨订单信息</returns>
        [McpServerTool, Description("更新披萨订单信息")]
        public PizzaOrderSessionDto UpdatePizzaOrderSession(
            [Description("披萨订单Id"), Required] string orderId,
            [Description("新披萨订单信息"), Required] PizzaOrderSessionDto session
        ) {
            if (_orders.TryGetValue(orderId, out var existingSession)) {
                //更新订单信息
                existingSession.PizzaType = session.PizzaType;
                existingSession.Quantity = session.Quantity;
                existingSession.PaymentMethod = session.PaymentMethod;
                existingSession.TotalPrice = session.TotalPrice;
                existingSession.IsPaid = false;

                return existingSession;
            } else {
                throw new ArgumentException($"订单Id {orderId} 不存在");
            }
        }

        /// <summary>
        /// 开始一个披萨订单
        /// </summary>
        /// <param name="pizzaType">披萨类型</param>
        /// <param name="quantity">数量</param>
        /// <returns>结果</returns>
        [McpServerTool, Description("开始一个披萨订单（必须提供披萨类型和订购数量，不允许假设数量为1）")]
        public PizzaOrderResultDto StartPizzaOrder(
            [Description("披萨类型，例如：奶酪、夏威夷"), Required] string pizzaType,
            [Description("订购数量,用户必须明确输入订购数量"), Required] int? quantity
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
            [Description("支付方式，如：支付宝、微信、信用卡"), Required] string paymentMethod,
            [Description("支付金额，用户必须明确提供支付金额"), Required] decimal? totalPrice
        ) {
            if (!_orders.TryGetValue(orderId, out var session)) {
                throw new ArgumentException($"订单Id {orderId} 不存在");
            } else if ((totalPrice ?? 0) <= 0) {
                throw new ArgumentOutOfRangeException(nameof(totalPrice), "支付金额，用户必须明确提供支付金额");
            }

            //支付
            session.PaymentMethod = paymentMethod;
            session.TotalPrice = totalPrice;
            session.IsPaid = true;

            //结果
            return new PizzaPaymentResultDto {
                OrderId = orderId,
                Message = $"订单 {orderId} 已支付成功，支付方式：{paymentMethod}，金额：{totalPrice} 元",
                PaymentMethod = paymentMethod,
                PaidAmount = totalPrice
            };
        }
    }
}
