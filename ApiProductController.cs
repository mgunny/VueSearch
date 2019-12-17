using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SilverlineWolseleyContentful.DataAccess.ContentfulModels;
using SilverlineWolseleyContentful.DataAccess.Repository;
using SilverlineWolseleyContentful.Models;
using Westwind.AspNetCore.Markdown;

namespace SilverlineWolseleyContentful.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiProductController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;
        private readonly AppSettings _appSettings;

        public ApiProductController(IDataRepository dataRepository, IOptions<AppSettings> appSettings)
        {
            _dataRepository = dataRepository;
            _appSettings = appSettings.Value;
        }

        [HttpGet("MainCategories")]
        public async Task<IActionResult> MainCategoriesAsync(string culture, bool includeSubCategories = true)
        {
            try
            {
                var data = await _dataRepository.GetMainCategoriesAsync(culture);
                ContentfulCollection<SubCategory> subCatgs = new ContentfulCollection<SubCategory>();

                if (includeSubCategories) { subCatgs = await _dataRepository.GetSubCategoriesAsync(culture); }

                var viewModel = new List<ProductHomeMainCategoryViewModel>();
                foreach (var mc in data.Items.OrderBy(m=> m.DisplayOrder))
                {
                    ProductHomeMainCategoryViewModel vm = new ProductHomeMainCategoryViewModel() { MainCategory = mc };
                    if (includeSubCategories)
                    {
                        vm.SubCategories = subCatgs.Where(s => s.MainCategory?.Sys.Id == mc.Sys.Id).OrderBy(o => o.DisplayOrder).ToList();
                    }
                    viewModel.Add(vm);
                }

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ProductsForSubCategory")]
        public async Task<IActionResult> ProductsForSubCategoryAsync(string culture, string subCategoryId)
        {
            try
            {
                var data = await _dataRepository.GetProductsAsync(culture, subCategoryId);
                return Ok(data.Items.OrderBy(p => p.PrimaryDescription).ThenBy(s => s.SecondaryDescription));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ProductDetails")]
        public async Task<IActionResult> ProductDetailAsync(string culture, string productId)
        {
            try
            {
                var data = await _dataRepository.GetProductDetailsAsync(culture, productId);

                // Parse any MarkDown fields into html
                //data.Specification = Markdown.Parse(data.Specification);
                //data.ProductFeatures = Markdown.Parse(data.ProductFeatures);
                
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("FeaturedProducts")]
        public async Task<IActionResult> FeaturedProductsAsync(string culture)
        {
            try
            {
                var data = await _dataRepository.GetFeaturedProductsAsync(culture);
                return Ok(data.Items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("NewProducts")]
        public async Task<IActionResult> NewProductsAsync(string culture)
        {
            try
            {
                var entryId = _appSettings.General.NewProductsEntryId;
                var data = await _dataRepository.GetCustomProductPageAsync(culture, entryId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ProductSearch")]
        public async Task<IActionResult> ProductSearchAsync(string culture, string searchText)
        {
            try
            {
                var entryId = _appSettings.General.NewProductsEntryId;
                var data = await _dataRepository.GetProductsForSearchAsync(culture, searchText);
                return Ok(data.Items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ShoppingCart")]
        public async Task<IActionResult> ShoppingCartAsync(string culture, string productSkus)
        {
            try
            {
                var productList = productSkus.Split(",").ToList();
                var products = await _dataRepository.GetProductsForShoppingCartAsync(culture, productList);
                return Ok(products.Items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}