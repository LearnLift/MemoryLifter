/*
* Copyright (c) 2007, KO Software (official@koapproach.com)
*
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*
*     * All original and modified versions of this source code must include the
*       above copyright notice, this list of conditions and the following
*       disclaimer.
*
*     * This code may not be used with or within any modules or code that is 
*       licensed in any way that that compels or requires users or modifiers
*       to release their source code or changes as a requirement for
*       the use, modification or distribution of binary, object or source code
*       based on the licensed source code. (ex: Cannot be used with GPL code.)
*
*     * The name of KO Software may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY KO SOFTWARE ``AS IS'' AND ANY EXPRESS OR
* IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
* OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
* EVENT WILL KO SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; 
* OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
* WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
* OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
* ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Security;
using System.Security.Permissions;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Resources;

using Kerido.Controls;

namespace Kerido.Controls.Design
{
	public delegate object TransactionAwareParammedMethod(IDesignerHost theHost, object theParam);

	public abstract class DesignerTransactionUtility
	{
		public static object DoInTransaction(
			IDesignerHost theHost,
			string theTransactionName,
			TransactionAwareParammedMethod theMethod,
			object theParam)
		{
			DesignerTransaction aTran = null;
			object aRetVal = null;

			try
			{
				aTran = theHost.CreateTransaction(theTransactionName);

				aRetVal = theMethod(theHost, theParam);      // do the task polymorphically
			}
			catch (CheckoutException theEx)	// something has gone wrong with transaction creation
			{
				if (theEx != CheckoutException.Canceled)
					throw theEx;
			}
			catch
			{
				if (aTran != null)
				{
					aTran.Cancel();
					aTran = null;	// the transaction won't commit in the finally block
				}

				throw;
			}
			finally
			{
				if (aTran != null)
					aTran.Commit();
			}

			return aRetVal;
		}
	}
}