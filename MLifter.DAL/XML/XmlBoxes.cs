using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// The IBoxes implementation for XML learning modules.
	/// </summary>
	public class XmlBoxes : IBoxes
	{
		private XmlDictionary m_Dictionary;
		private List<IBox> m_Box;

		internal XmlBoxes(XmlDictionary dictionary, ParentClass parent)
		{
			Parent = parent;
			m_Dictionary = dictionary;

			m_Box = new List<IBox>(dictionary.NumberOfBoxes + 1);
			for (int i = 0; i < dictionary.NumberOfBoxes + 1; i++) //including pool and box 10
			{
				m_Box.Add(new XmlBox(m_Dictionary, i, Parent.GetChildParentClass(this)));
			}
			m_Box[0].MaximalSize = Int32.MaxValue;
			m_Box[dictionary.NumberOfBoxes].MaximalSize = Int32.MaxValue;
		}

		#region IBoxes Members

		/// <summary>
		/// Gets the box.
		/// </summary>
		public List<IBox> Box
		{
			get { return m_Box; }
		}

		#endregion

		/// <summary>
		/// Updates all boxes in this instance.
		/// </summary>
		public void Update()
		{
			foreach (IBox box in m_Box)
				((XmlBox)box).UpdateQuerySize();
		}


		#region IParent Members

		/// <summary>
		/// Gets the parent.
		/// </summary>
		public ParentClass Parent { get; private set; }

		#endregion
	}
}
