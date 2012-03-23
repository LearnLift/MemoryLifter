/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
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
