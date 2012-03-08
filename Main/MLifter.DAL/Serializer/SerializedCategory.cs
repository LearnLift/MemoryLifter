using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MLifter.DAL
{
    [Serializable]
    public class SerializedCategory
    {
        private int _id = -1;
        private string _description = "";

        private SerializedCategory() 
        {
            // for serialization only!
        }

        public SerializedCategory(int id)
        {
            _id = id;
        }

        public SerializedCategory(int id, string description)
        {
            _id = id;
            _description = description;
        }

        [XmlText]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlIgnore]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}