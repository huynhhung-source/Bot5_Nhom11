using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using doanweb.Models;

namespace doanweb.Services
{
    public interface IInvoiceService
    {
        byte[] GenerateInvoicePdf(Order order, List<OrderItem> orderItems);
    }

    public class InvoiceService : IInvoiceService
    {
        private const string PrimaryColor = "#003da5";
        private const string AccentColor = "#f36100";

        public byte[] GenerateInvoicePdf(Order order, List<OrderItem> orderItems)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var items = orderItems ?? new List<OrderItem>();
            var customerName = order.User?.FullName ?? "Khách hàng";
            var customerEmail = order.User?.Email ?? "";
            var customerPhone = order.User?.PhoneNumber ?? "";
            var deliveryAddress = string.IsNullOrWhiteSpace(order.DeliveryAddress)
                ? ""
                : order.DeliveryAddress.Trim();
            var orderStatus = TranslateOrderStatus(order.Status);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(style => style.FontSize(10));

                    page.Header().Element(header =>
                    {
                        header.Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("GYM CENTER")
                                    .FontSize(24).Bold().FontColor(PrimaryColor);
                                col.Item().Text("Hóa đơn thanh toán")
                                    .FontSize(12).FontColor(Colors.Grey.Darken2);
                            });

                            row.ConstantItem(200).AlignRight().Column(col =>
                            {
                                col.Item().Text($"Đơn hàng #{order.OrderId}")
                                    .Bold().FontSize(12);
                                col.Item().Text($"Ngy: {order.OrderDate:dd/MM/yyyy HH:mm}")
                                    .FontSize(10);
                            });
                        });
                    });

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Spacing(12);

                        column.Item().Text("Thông tin khách hàng")
                            .Bold().FontSize(12).FontColor(PrimaryColor);
                        column.Item().Text($"Khách hàng: {customerName}");
                        column.Item().Text($"Email: {customerEmail}");
                        column.Item().Text($"điện thoại: {customerPhone}");
                        column.Item().Text($"địa chỉ giao hàng: {deliveryAddress}");

                        column.Item().PaddingTop(8).Text("Chi tiết sản phẩm")
                            .Bold().FontSize(12).FontColor(PrimaryColor);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(6)
                                    .Text("Sản phẩm").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(6)
                                    .AlignCenter().Text("SL").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(6)
                                    .AlignRight().Text("Đơn giá").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(6)
                                    .AlignRight().Text("Thành tiền").Bold();
                            });

                            if (items.Count == 0)
                            {
                                table.Cell().ColumnSpan(4).Padding(8)
                                    .Text("Không có sản phẩm trong đơn hàng").Italic();
                            }
                            else
                            {
                                foreach (var item in items)
                                {
                                    var productName = item.Product?.ProductName
                                        ?? $"Sản phẩm #{item.ProductId}";

                                    table.Cell().Padding(6).Text(productName);
                                    table.Cell().Padding(6).AlignCenter()
                                        .Text(item.Quantity.ToString());
                                    table.Cell().Padding(6).AlignRight()
                                        .Text(FormatMoney(item.UnitPrice));
                                    table.Cell().Padding(6).AlignRight()
                                        .Text(FormatMoney(item.TotalPrice));
                                }
                            }
                        });

                        column.Item().AlignRight().Width(220).Table(summary =>
                        {
                            summary.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1f);
                            });

                            summary.Cell().Padding(4).Text("Tổng tiền:").Bold();
                            summary.Cell().Padding(4).AlignRight()
                                .Text(FormatMoney(order.TotalAmount));

                            summary.Cell().Padding(4).Text("Phí vận chuyển:").Bold();
                            summary.Cell().Padding(4).AlignRight().Text("Miễn Phí");

                            summary.Cell().Background(Colors.Orange.Lighten4).Padding(8)
                                .Text("Thanh Toán:").Bold().FontSize(12);
                            summary.Cell().Background(Colors.Orange.Lighten4).Padding(8)
                                .AlignRight().Text(FormatMoney(order.TotalAmount))
                                .Bold().FontSize(12).FontColor(AccentColor);
                        });

                        column.Item().Text($"Trạng thái đơn hàng: {orderStatus}").Bold();
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("GYM CENTER  ").FontSize(9).FontColor(Colors.Grey.Medium);
                        text.Span(DateTime.Now.Year.ToString()).FontSize(9).FontColor(Colors.Grey.Medium);
                        text.Span(" | Cảm ơn quý khách mua hàng!").FontSize(9).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static string FormatMoney(decimal amount) =>
            $"{amount:N0} ?";

        private static string TranslateOrderStatus(string? status) => status switch
        {
            "Pending" => "Chờ xử lý",
            "Confirmed" => "xác nhận ",
            "Processing" => "trạng thái xử lý",
            "Completed" => "Hoàn thành",
            "Shipped" => "Đang giao",
            "Delivered" => "Đã giao",
            "Cancelled" => "Đã hủy",
            _ => status ?? ""
        };
    }
}
