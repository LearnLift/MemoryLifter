using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Tools;

namespace MLifter.DAL.XML
{
	/// <summary>
	/// The IQueryType implementation for XML learning modules.
	/// </summary>
	public class XmlQueryType : IQueryType
	{
		XmlDocument m_dictionary;
		const string m_xpath = "/dictionary/user/";
		const string m_xpathQueryType = "querytype";

		internal XmlQueryType(XmlDocument dictionary, ParentClass parent)
		{
			m_dictionary = dictionary;
			Parent = parent;
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
			return QueryTypeHelper.Compare(this, obj);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region IQueryType Members

		/// <summary>
		/// Gets or sets if the image recognition query mode is possible.
		/// </summary>
		/// <value>
		/// The image recognition query mode is possible.
		/// </value>
		public bool? ImageRecognition
		{
			get
			{
				return Check(EQueryType.ImageRecognition);
			}
			set
			{
				if (value.GetValueOrDefault())
					Set(EQueryType.ImageRecognition);
				else
					Unset(EQueryType.ImageRecognition);
			}
		}

		/// <summary>
		/// Gets or sets if the listening comprehension query mode is possible.
		/// </summary>
		/// <value>
		/// The listening comprehension query mode is possible.
		/// </value>
		public bool? ListeningComprehension
		{
			get
			{
				return Check(EQueryType.ListeningComprehension);
			}
			set
			{
				if (value.GetValueOrDefault())
					Set(EQueryType.ListeningComprehension);
				else
					Unset(EQueryType.ListeningComprehension);
			}
		}

		/// <summary>
		/// Gets or sets if the multiple choice query mode is possible.
		/// </summary>
		/// <value>
		/// The multiple choice query mode is possible.
		/// </value>
		public bool? MultipleChoice
		{
			get
			{
				return Check(EQueryType.MultipleChoice);
			}
			set
			{
				if (value.GetValueOrDefault())
					Set(EQueryType.MultipleChoice);
				else
					Unset(EQueryType.MultipleChoice);
			}
		}

		/// <summary>
		/// Gets or sets if the sentence query mode is possible.
		/// </summary>
		/// <value>
		/// The sentence query mode is possible.
		/// </value>
		public bool? Sentence
		{
			get
			{
				return Check(EQueryType.Sentences);
			}
			set
			{
				if (value.GetValueOrDefault())
					Set(EQueryType.Sentences);
				else
					Unset(EQueryType.Sentences);
			}
		}

		/// <summary>
		/// Gets or sets if the word query mode is possible.
		/// </summary>
		/// <value>
		/// The word query mode is possible.
		/// </value>
		public bool? Word
		{
			get
			{
				return Check(EQueryType.Word);
			}
			set
			{
				if (value.GetValueOrDefault())
					Set(EQueryType.Word);
				else
					Unset(EQueryType.Word);
			}
		}

		#endregion

		/// <summary>
		/// Sets the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		private void Set(EQueryType type)
		{
			XmlConfigHelper.Set(m_dictionary, m_xpath + m_xpathQueryType, (int)type);
		}

		/// <summary>
		/// Unsets the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		private void Unset(EQueryType type)
		{
			XmlConfigHelper.Unset(m_dictionary, m_xpath + m_xpathQueryType, (int)type);
		}

		/// <summary>
		/// Checks the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		private bool Check(EQueryType type)
		{
			return XmlConfigHelper.Check(m_dictionary, m_xpath + m_xpathQueryType, (int)type);
		}


		#region ICopy Members

		/// <summary>
		/// Copies this instance to the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="progressDelegate">The progress delegate.</param>
		public void CopyTo(MLifter.DAL.Tools.ICopy target, CopyToProgress progressDelegate)
		{
			CopyBase.Copy(this, target, typeof(IQueryType), progressDelegate);
		}

		#endregion

		#region IParent Members

		/// <summary>
		/// Gets the parent.
		/// </summary>
		public ParentClass Parent { get; private set; }

		#endregion
	}
}
