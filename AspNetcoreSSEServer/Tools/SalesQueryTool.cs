﻿using AspNetcoreSSEServer.Application.ViewModel;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace AspNetcoreSSEServer.Tools {
    /// <summary>
    /// SalesQueryTool - A tool for querying sales data
    /// </summary>
    [McpServerToolType, Description("销售数据查询工具")]
    public class SalesQueryTool {
        /// <summary>
        /// GetDailySales - Gets daily sales data for a specified date range
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>日销售数据集合</returns>
        [McpServerTool, Description("获取指定日期范围内的销售数据")]
        public IEnumerable<DailySalesViewModel> GetDailySales(
            [Description("开始日期")] DateTime startDate,
            [Description("结束日期")] DateTime endDate
        ) {
            var salesData = new List<DailySalesViewModel>();

            // Generate random sales data for the specified date range
            for (var date = startDate; date <= endDate; date = date.AddDays(1)) {
                salesData.AddRange([
                    new() {
                        Date = date,
                        WaresName = "商品" + new Random().Next(1, 10).ToString(),
                        TotalSales = new Random().Next(1000, 5000)
                    },
                    new() {
                        Date = date,
                        WaresName = "商品" + new Random().Next(1, 10).ToString(),
                        TotalSales = new Random().Next(1000, 5000)
                    }
                ]);
            }

            return salesData;
        }
    }
}
