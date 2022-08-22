using System.Collections.Generic;

namespace RecipeBox.Models
{
    public class Recipe
    {
        public Recipe()
        {
            this.JoinEntities = new HashSet<RecipeTag>();
        }

        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string Ingredients { get; set; }
        public string Instruction { get; set; }
        public string UserId { get; set; } //Foreign Key
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<RecipeTag> JoinEntities { get; }
    }
}
