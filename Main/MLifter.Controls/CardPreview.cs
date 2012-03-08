using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MLifter.BusinessLayer;
using MLifter.Controls.Properties;

namespace MLifter.Controls
{
    public partial class CardPreview : Form
    {
        Card card;
        Dictionary dic;

        public CardPreview(Card cardToDisplay, Dictionary dictionary)
        {
            card = cardToDisplay;
            dic = dictionary;

            InitializeComponent();
        }

        private void CardPreview_Load(object sender, EventArgs e)
        {
            webBrowserAnswer.Url = MLifter.DAL.DB.DbMediaServer.DbMediaServer.PrepareAnswer(dic.DictionaryDAL.Parent, card.BaseCard.Id,
                dic.GenerateCard(card, MLifter.DAL.Interfaces.Side.Answer, Resources.XSLEDIT_USERANSWER, true));
            webBrowserQuestion.Url = MLifter.DAL.DB.DbMediaServer.DbMediaServer.PrepareQuestion(dic.DictionaryDAL.Parent, card.BaseCard.Id,
                dic.GenerateCard(card, MLifter.DAL.Interfaces.Side.Question, string.Empty, true));
        }

        private void CardPreview_FormClosing(object sender, FormClosingEventArgs e)
        {
            webBrowserAnswer.Stop();
            webBrowserQuestion.Stop();
        }
    }
}