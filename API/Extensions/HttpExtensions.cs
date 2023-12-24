using System.Text.Json;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        //Ajout d'un header pour la pagination
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {   //Creation d'une instance de la classe PaginationHeader
            var paginationHeader= new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            //Creation d'une instance de la classe JsonSerializerOptions
            var options= new JsonSerializerOptions
            {
                PropertyNamingPolicy= JsonNamingPolicy.CamelCase
            };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

    }
}