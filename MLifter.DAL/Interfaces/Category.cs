using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL
{
	/// <summary>
	/// This class represents a learning module category.
	/// </summary>
	[Serializable()]
	public class Category
	{
		private bool m_Converted = false;
		private int m_Id = -1;
		//private int m_OldId = -1;
		private string m_Description = "";

		internal event EventHandler IdChanged;

		/// <summary>
		/// The id of the default category.
		/// </summary>
		public static readonly int DefaultCategory = 3;

		/// <summary>
		/// The names of the legacy categories.
		/// </summary>s
		private readonly string[] OldCategoryNames = new string[12]
		{
			Properties.Resources.CATEGORYNAMES0, 
			Properties.Resources.CATEGORYNAMES1, 
			Properties.Resources.CATEGORYNAMES2, 
			Properties.Resources.CATEGORYNAMES3, 
			Properties.Resources.CATEGORYNAMES4, 
			Properties.Resources.CATEGORYNAMES5,
			Properties.Resources.CATEGORYNAMES6, 
			Properties.Resources.CATEGORYNAMES7, 
			Properties.Resources.CATEGORYNAMES8, 
			Properties.Resources.CATEGORYNAMES9, 
			Properties.Resources.CATEGORYNAMES10,
			Properties.Resources.CATEGORYNAMES11
		};

		private readonly string[] CategoryNames = new string[6]
		{
			Properties.Resources.NEWCATEGORYNAMES0, 
			Properties.Resources.NEWCATEGORYNAMES1, 
			Properties.Resources.NEWCATEGORYNAMES2, 
			Properties.Resources.NEWCATEGORYNAMES3, 
			Properties.Resources.NEWCATEGORYNAMES4, 
			Properties.Resources.NEWCATEGORYNAMES5,
		};

		private Category()
		{
			// for serialization only!
		}

		internal Category(int id)
		{
			m_Id = id;
			m_Converted = true;
		}

		internal Category(int id, bool converted)
		{
			m_Id = id;
			m_Converted = converted;
			if (!converted)
			{
				//automatically translate Arts, Languages and Miscellaneus
				switch (m_Id)
				{
					case 0:
						m_Id = 1;
						m_Converted = true;
						break;
					case 4:
						m_Id = 2;
						m_Converted = true;
						break;
					case 7:
						m_Id = 3;
						m_Converted = true;
						break;
					default:
						break;
				}
			}
		}

		private Category(int id, string description)
		{
			m_Id = id;
			m_Description = description;
		}

		internal bool Converted
		{
			get { return m_Converted; }
			set { m_Converted = value; }
		}

		/// <summary>
		/// Gets the old id.
		/// </summary>
		/// <value>The old id.</value>
		/// <remarks>Documented by Dev03, 2008-02-14</remarks>
		internal int OldId
		{
			get
			{
				//CATEGORYNAMES0	Arts
				//CATEGORYNAMES1	Earth Sciences
				//CATEGORYNAMES2	History
				//CATEGORYNAMES3	Humanities
				//CATEGORYNAMES4	Languages
				//CATEGORYNAMES5	Leisure
				//CATEGORYNAMES6	Medical Sciences
				//CATEGORYNAMES7	Miscellaneous
				//CATEGORYNAMES8	Politics
				//CATEGORYNAMES9	Pure Sciences
				//CATEGORYNAMES10	Social Science
				//CATEGORYNAMES11	Technology
				//NEWCATEGORYNAMES0	Applied Sciences (0)	->  Pure Sciences (9)
				//NEWCATEGORYNAMES1	Arts (1)	->  Arts (0)
				//NEWCATEGORYNAMES2	Languages (2)	->  Languages (4)
				//NEWCATEGORYNAMES3	Miscellaneous (3)	->  Miscellaneous (7)
				//NEWCATEGORYNAMES4	Natural Sciences (4)	->  Earth Sciences (1)
				//NEWCATEGORYNAMES5	Social Sciences (5) ->  Humanities (3)
				if (!m_Converted)
					return m_Id;
				int oldId = 7;
				switch (m_Id)
				{
					case 0:
						oldId = 9;
						break;
					case 1:
						oldId = 0;
						break;
					case 2:
						oldId = 4;
						break;
					case 4:
						oldId = 1;
						break;
					case 5:
						oldId = 3;
						break;
					case 3:
					default:
						oldId = 7;
						break;
				}
				return oldId;
			}
		}

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		public int Id
		{
			get { return m_Id; }
			set
			{
				m_Id = value;
				IdChanged(this, null);
			}
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		public string Description
		{
			get
			{
				if (m_Converted)
				{
					if ((m_Id >= 0) && (m_Id < CategoryNames.Length))
						return CategoryNames[m_Id];
					else
						return m_Description;
				}
				else
				{
					if ((m_Id >= 0) && (m_Id < OldCategoryNames.Length))
						return OldCategoryNames[m_Id];
					else
						return m_Description;
				}
			}
			set { m_Description = value; }
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return String.Format("{0}", Description);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj"/> parameter is null.
		///   </exception>
		public override bool Equals(Object obj)
		{
			if (obj == null)
				return false;

			Category category = obj as Category;
			if (category == null)
				return false;

			return Equals(category);
		}

		/// <summary>
		/// Checks if this category equalses to the specified category.
		/// </summary>
		/// <param name="category">The category to compare to.</param>
		/// <returns></returns>
		public bool Equals(Category category)
		{
			if (category == null)
				return false;

			return (this.Id == category.Id && this.Converted == category.Converted);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			return this.Converted ? this.Id : this.Id + CategoryNames.Length;
		}


		// STATIC Classes

		/// <summary>
		/// Gets the default category.
		/// </summary>
		/// <returns></returns>
		public static Category GetDefaultCategory()
		{
			return new Category(MLifter.DAL.Category.DefaultCategory);
		}

		/// <summary>
		/// Gets all categories.
		/// </summary>
		/// <returns></returns>
		public static List<Category> GetCategories()
		{
			List<Category> categories = new List<Category>();
			Category mother = new Category();
			for (int i = 0; i < mother.CategoryNames.Length; i++)
				categories.Add(new Category(i));
			return categories;
		}

		/// <summary>
		/// Converts an old CategoryId to the new CategoryId.
		/// </summary>
		/// <param name="oldCatId">old category id.</param>
		/// <returns>the new Category Id</returns>
		/// <remarks>Documented by Dev08, 2008-10-06</remarks>
		public static int ConvertCategoryId(int oldCatId)
		{
			switch (oldCatId)
			{
				case 0:             //Old Category "Arts"
					return 1;       //New Category "Arts"

				case 4:             //Old Category "Languages"
					return 2;       //New Category "Languages"

				case 6:             //Old Category "Miscellaneous"
					return 3;       //New Category "Miscellaneous"

				case 10:            //Old Category "Social Sciences"
					return 5;       //New Category "Social Sciences"

				default:
					return 3;       //New Category "Miscellaneous"
			}
		}
	}
}