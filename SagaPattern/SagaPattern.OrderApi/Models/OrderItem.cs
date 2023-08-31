using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SagaPattern.Choreography.OrderApi.Models;
public class OrderItem
{
    public OrderItem()
    {
        OrderId = 1;
        ProductId = 1;
        UnitPrice = 500;
    }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }
}