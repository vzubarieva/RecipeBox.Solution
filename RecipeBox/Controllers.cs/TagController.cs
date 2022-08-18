using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;

namespace RecipeBox.Controllers
{
    public class TagsController : Controller
    {
        private readonly RecipeBoxContext _db;

        public TagsController(RecipeBoxContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            return View(_db.Tags.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.RecipeId = new SelectList(_db.Recipes, "RecipeId", "RecipeName");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Tag tag, int RecipeId)
        {
            _db.Tags.Add(tag);
            _db.SaveChanges();
            if (RecipeId != 0)
            {
                _db.RecipeTag.Add(new RecipeTag() { RecipeId = RecipeId, TagId = tag.TagId });
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var thisTag = _db.Tags
                .Include(tag => tag.JoinEntities)
                .ThenInclude(join => join.Recipe)
                .FirstOrDefault(tag => tag.TagId == id);
            return View(thisTag);
        }

        public ActionResult Edit(int id)
        {
            var thisTag = _db.Tags.FirstOrDefault(tag => tag.TagId == id);
            ViewBag.RecipeId = new SelectList(_db.Recipes, "RecipeId", "RecipeName");
            return View(thisTag);
        }

        [HttpPost]
        public ActionResult Edit(Tag tag, int RecipeId)
        {
            if (RecipeId != 0)
            {
                _db.RecipeTag.Add(new RecipeTag() { RecipeId = RecipeId, TagId = tag.TagId });
            }
            _db.Entry(tag).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddRecipe(int id)
        {
            var thisTag = _db.Tags.FirstOrDefault(tag => tag.TagId == id);
            ViewBag.RecipeId = new SelectList(_db.Recipes, "RecipeId", "RecipeName");
            return View(thisTag);
        }

        [HttpPost]
        public ActionResult AddRecipe(Tag tag, int RecipeId)
        {
            if (RecipeId != 0)
            {
                _db.RecipeTag.Add(new RecipeTag() { RecipeId = RecipeId, TagId = tag.TagId });
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var thisTag = _db.Tags.FirstOrDefault(tag => tag.TagId == id);
            return View(thisTag);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var thisTag = _db.Tags.FirstOrDefault(tag => tag.TagId == id);
            _db.Tags.Remove(thisTag);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteRecipe(int joinId)
        {
            var joinEntry = _db.RecipeTag.FirstOrDefault(entry => entry.RecipeTagId == joinId);
            _db.RecipeTag.Remove(joinEntry);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
