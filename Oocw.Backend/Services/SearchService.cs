

using Microsoft.AspNetCore.Mvc;

namespace Oocw.Backend.Services;

public class SearchService
{

    [FromServices] public DatabaseService DbService { get; set; } = null!;



    

}