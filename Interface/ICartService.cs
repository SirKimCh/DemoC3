using BanhMyIT.Models;using System.Threading.Tasks;namespace BanhMyIT.Interface{ public interface ICartService { Task<Cart> GetOrCreateCartAsync(int userId); Task<Cart?> GetCartWithItemsAsync(int userId); Task AddItemAsync(int userId,int productId,int quantity); Task RemoveItemAsync(int cartDetailId); Task ClearCartAsync(int userId); Task<Bill> CheckoutAsync(int userId, PayMethod? payMethod); }}

