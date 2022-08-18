using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;

namespace RecipeBox.Controllers
{
    public class RecipesController : Controller
    {
        private readonly RecipeBoxContext _db;

        public RecipesController(RecipeBoxContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            return View(_db.Recipes.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.TagId = new SelectList(_db.Tags, "TagId", "RecipeCategory");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Recipe recipe, int TagId)
        {
            _db.Recipes.Add(recipe);
            _db.SaveChanges();
            if (TagId != 0)
            {
                _db.RecipeTag.Add(new RecipeTag() { TagId = TagId, RecipeId = recipe.RecipeId });
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var thisRecipe = _db.Recipes
                .Include(recipe => recipe.JoinEntities)
                .ThenInclude(join => join.Tag)
                .FirstOrDefault(recipe => recipe.RecipeId == id);
            return View(thisRecipe);
        }

        public ActionResult Edit(int id)
        {
            var thisRecipe = _db.Recipes.FirstOrDefault(recipe => recipe.RecipeId == id);
            ViewBag.TagId = new SelectList(_db.Tags, "TagId", "RecipeCategory");
            return View(thisRecipe);
        }

        [HttpPost]
        public ActionResult Edit(Recipe recipe, int TagId)
        {
            if (TagId != 0)
            {
                _db.RecipeTag.Add(new RecipeTag() { TagId = TagId, RecipeId = recipe.RecipeId });
            }
            _db.Entry(recipe).State = EntityState.Modified;
            _db.SaveChanges();
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
