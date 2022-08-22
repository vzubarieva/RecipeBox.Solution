using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace RecipeBox.Controllers
{
    public class RecipesController : Controller
    {
        private readonly RecipeBoxContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public RecipesController(
            UserManager<ApplicationUser> userManager,
            RecipeBoxContext db,
            ILogger<RecipesController> logger
        )
        {
            _userManager = userManager;
            _db = db;
            _logger = logger;
        }

        public ActionResult Index()
        {
            var recipes = _db.Recipes.ToList();
            _logger.LogInformation("recipes Index() method");

            return View(recipes);
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewBag.TagId = new SelectList(_db.Tags, "TagId", "RecipeCategory");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Recipe recipe, int TagId)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //find value of currently logged-in user.
            var currentUser = await _userManager.FindByIdAsync(userId);
            recipe.User = currentUser; //associate current user to recipe's user property.
            _db.Recipes.Add(recipe); //adds recipes to the RecipesDbSet
            _db.SaveChanges(); //save changes to database object called DB or _db
            if (TagId != 0)
            {
                _db.RecipeTag.Add(new RecipeTag() { TagId = TagId, RecipeId = recipe.RecipeId });
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        } //Add() is a method we run on our DBSet property of our DBContext, while SaveChanges() is a method we run on the DBContext itself.

        //Together, they update the DBSet and then sync those changes to the database which the DBContext represents.

        public async Task<ActionResult> Details(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //find value of currently logged-in user.
            var currentUser = await _userManager.FindByIdAsync(userId);

            var thisRecipe = _db.Recipes
                .Include(recipe => recipe.JoinEntities)
                .ThenInclude(join => join.Tag)
                .FirstOrDefault(recipe => recipe.RecipeId == id);

            ViewBag.isOwner = thisRecipe.UserId == currentUser.Id;

            return View(thisRecipe);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var thisRecipe = _db.Recipes
                .Include(recipe => recipe.User)
                .FirstOrDefault(recipe => recipe.RecipeId == id);
            ViewBag.TagId = new SelectList(_db.Tags, "TagId", "RecipeCategory");

            return View(thisRecipe);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit(Recipe recipe, int TagId)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //find value of currently logged-in user.
            var currentUser = await _userManager.FindByIdAsync(userId);
            _logger.LogInformation("-----------");
            _logger.LogInformation(recipe.UserId);
            _logger.LogInformation(currentUser.Id);

            if (recipe.UserId == currentUser.Id)
            {
                if (TagId != 0)
                {
                    _db.RecipeTag.Add(
                        new RecipeTag() { TagId = TagId, RecipeId = recipe.RecipeId }
                    );
                }
                _db.Entry(recipe).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult AddTag(int id)
        {
            var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
            ViewBag.TagId = new SelectList(_db.Tags, "TagId", "RecipeCategory");
            return View(thisRecipe);
        }

        [HttpPost]
        public ActionResult AddTag(Recipe recipe, int TagId)
        {
            if (TagId != 0)
            {
                _db.RecipeTag.Add(new RecipeTag() { TagId = TagId, RecipeId = recipe.RecipeId });
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
            return View(thisRecipe);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
            _db.Recipes.Remove(thisRecipe);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteTag(int joinId)
        {
            var joinEntry = _db.RecipeTag.FirstOrDefault(entry => entry.RecipeTagId == joinId);
            _db.RecipeTag.Remove(joinEntry);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
