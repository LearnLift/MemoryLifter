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
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public class MultiPaneControlDesigner : ParentControlDesigner
	{
		#region Fields
		private DesignerVerbCollection myVerbs;

		private DesignerVerb
			myRemoveVerb,
			myAddVerb,
			mySwitchVerb;

		MultiPanePage mySelectedPage;

		bool myInTransaction = false;
		#endregion

		#region Methods - own
		private void CheckVerbStatus()
		{
			if (Control == null)
				myRemoveVerb.Enabled = myAddVerb.Enabled = mySwitchVerb.Enabled = false;
			else
			{
				myAddVerb.Enabled = true;
				myRemoveVerb.Enabled = (Control.Controls.Count > 1);
				mySwitchVerb.Enabled = (Control.Controls.Count > 1);
			}
		}

		private MultiPanePageDesigner GetSelectedPageDesigner()
		{
			MultiPanePage aSelPage = mySelectedPage;

			if (aSelPage == null)
				return null;

			MultiPanePageDesigner aDesigner = null;

			IDesignerHost aSrv = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (aSrv != null)
				aDesigner = (MultiPanePageDesigner)aSrv.GetDesigner(aSelPage);

			return aDesigner;
		}

		private static MultiPanePage GetPageOfControl(object theControl)
		{
			if (!(theControl is Control))
				return null;

			Control aParent = (Control)theControl;

			// iterate through parent hierarchy until we reach 
			// a MultiPanePage that theControl sits upon
			while ((aParent != null) && !(aParent is MultiPanePage))
				aParent = aParent.Parent;

			return (MultiPanePage)aParent;
		}
		#endregion
		
		#region Methods - own - Designer Transaction bodies

		private object Transaction_AddPage(IDesignerHost theHost, object theParam)
		{
			MultiPaneControl aCtl = DesignedControl;

			MultiPanePage aNewPage = (MultiPanePage)theHost.CreateComponent(typeof(MultiPanePage));

			MemberDescriptor aMem_Controls = TypeDescriptor.GetProperties(aCtl)["Controls"];

			RaiseComponentChanging(aMem_Controls);

			aCtl.Controls.Add(aNewPage);
			DesignerSelectedPage = aNewPage;

			RaiseComponentChanged(aMem_Controls, null, null);

			return null;
		}

		private object Transaction_RemovePage(IDesignerHost theHost, object theParam)
		{
			if (mySelectedPage == null)
				return null;

			MultiPaneControl aCtl = DesignedControl;

			MemberDescriptor aMember_Controls = TypeDescriptor.GetProperties(aCtl)["Controls"];

			RaiseComponentChanging(aMember_Controls);

			try
			{
				theHost.DestroyComponent(mySelectedPage);
				// NOTE: we don't process anything here because processing will
				// be performed in Handler_ComponentRemoving
			}
			catch { }

			RaiseComponentChanged(aMember_Controls, null, null);

			return null;
		}

		private object Transaction_UpdateSelectedPage(IDesignerHost theHost, object theParam)
		{
			MultiPaneControl aCtl = DesignedControl;
			MultiPanePage aPgTemp = mySelectedPage;

			int aCurIndex = aCtl.Controls.IndexOf(mySelectedPage);

			if (mySelectedPage == aCtl.SelectedPage)	//we also need to update the SelectedPage property
			{
				MemberDescriptor aMember_SelectedPage = TypeDescriptor.GetProperties(aCtl)["SelectedPage"];

				RaiseComponentChanging(aMember_SelectedPage);

				if (aCtl.Controls.Count > 1)
				{
					// begin update current page
					if (aCurIndex == aCtl.Controls.Count - 1)                // NOTE: after SelectedPage has
						aCtl.SelectedPage = (MultiPanePage) aCtl.Controls[aCurIndex - 1];      // been updated, mySelectedPage
					else                                                  // has also changed
						aCtl.SelectedPage = (MultiPanePage) aCtl.Controls[aCurIndex + 1];
					// end update current page
				}
				else
					aCtl.SelectedPage = null;

				RaiseComponentChanged(aMember_SelectedPage, null, null);
			}
			else
			{
				if (aCtl.Controls.Count > 1)
				{
					if (aCurIndex == aCtl.Controls.Count - 1)
						DesignerSelectedPage = (MultiPanePage) aCtl.Controls[aCurIndex - 1];
					else
						DesignerSelectedPage = (MultiPanePage) aCtl.Controls[aCurIndex + 1];
				}
				else
					DesignerSelectedPage = null;
			}

			return null;
		}

		private object Transaction_SetSelectedPageAsConcrete(IDesignerHost theHost, object theParam)
		{
			MultiPaneControl aCtl = DesignedControl;

			MemberDescriptor aMember_SelectedPage = TypeDescriptor.GetProperties(aCtl)["SelectedPage"];

			RaiseComponentChanging(aMember_SelectedPage);
			aCtl.SelectedPage = (MultiPanePage)theParam;
			RaiseComponentChanged(aMember_SelectedPage, null, null);

			return null;
		}
		#endregion

		#region Event handlers

		private void Handler_SelectedPageChanged(object theSender, EventArgs theArgs)
		{
			mySelectedPage = DesignedControl.SelectedPage;

			// We do not call the property here, because the control
			// has already updated pages' visibility. We only must
			// sync the field with the control's current selection
		}


		private void Handler_AddPage(object theSender, EventArgs theArgs)
		{
			IDesignerHost aHost = (IDesignerHost)GetService(typeof(IDesignerHost));

			if (aHost == null)
				return;

			myInTransaction = true;
			DesignerTransactionUtility.DoInTransaction
			(
				aHost,
				"MultiPaneControlAddPage",
				new TransactionAwareParammedMethod(Transaction_AddPage),
				null
			);
			myInTransaction = false;
		}

		private void Handler_RemovePage(object sender, EventArgs eevent)
		{
			MultiPaneControl aCtl = DesignedControl;

			if (aCtl == null)
				return;

			else if (aCtl.Controls.Count < 1)
				return;

			IDesignerHost aHost = (IDesignerHost)GetService(typeof(IDesignerHost));

			if (aHost == null)
				return;

			myInTransaction = true;
			DesignerTransactionUtility.DoInTransaction
			(
				aHost,
				"MultiPaneControlRemovePage",
				new TransactionAwareParammedMethod(Transaction_RemovePage),
				null
			);
			myInTransaction = false;
		}

		private void Handler_SwitchPage(object theSender, EventArgs theArgs)
		{
			frmSwitchPages aForm = new frmSwitchPages(this);
			DialogResult aRes = aForm.ShowDialog();

			if (aRes != DialogResult.OK)
				return;

			if (aForm.SetSelectedPage)
			{
				IDesignerHost aHost = (IDesignerHost)GetService(typeof(IDesignerHost));

				if ( aHost != null)
					DesignerTransactionUtility.DoInTransaction
					(
						aHost,
						"MultiPaneControlSetSelectedPageAsConcrete",
						new TransactionAwareParammedMethod(Transaction_SetSelectedPageAsConcrete),
						aForm.FutureSelection
					);
			}
			else
				DesignerSelectedPage = aForm.FutureSelection;
		}

		private void Handler_ComponentChanged(object theSender, ComponentChangedEventArgs theArgs)
		{
			CheckVerbStatus();
		}

		private void Handler_ComponentRemoving(object theSender, ComponentEventArgs theArgs)
		{
			if ( !(theArgs.Component is MultiPanePage) )
				return;

			MultiPaneControl aCtl = DesignedControl;
			MultiPanePage aPg = (MultiPanePage)theArgs.Component;

			if ( !aCtl.Controls.Contains(aPg) )
				return;

			IDesignerHost aHost = (IDesignerHost)GetService(typeof(IDesignerHost));

			if (!myInTransaction)
			{
				myInTransaction = true;
				DesignerTransactionUtility.DoInTransaction
				(
					aHost,
					"MultiPaneControlRemoveComponent",
					new TransactionAwareParammedMethod(Transaction_UpdateSelectedPage),
					null
				);
				myInTransaction = false;
			}
			else
				Transaction_UpdateSelectedPage(aHost, null);

			CheckVerbStatus();
		}

		private void Handler_SelectionChanged(object sender, EventArgs e)
		{
			ISelectionService aSrv = (ISelectionService)GetService(typeof(ISelectionService));

			if (aSrv == null)
				return;

			ICollection aSel = aSrv.GetSelectedComponents();
			MultiPaneControl aCtl = DesignedControl;

			foreach (object aIt in aSel)
			{
				MultiPanePage aPage = GetPageOfControl(aIt);
				if ((aPage != null) && (aPage.Parent == aCtl))
				{
					DesignerSelectedPage = aPage;
					break;
				}
			}
		}

		#endregion

		#region Method everrides - drag-and-drop

		// Redirect the drag-and-drop operations to the designer
		// of the currently selected page.

		protected override void OnDragDrop(DragEventArgs theDragEvents)
		{
			MultiPanePageDesigner aDsgn_Sel = GetSelectedPageDesigner();
			if (aDsgn_Sel != null)
				aDsgn_Sel.InternalOnDragDrop(theDragEvents);
		}

		protected override void OnDragEnter(DragEventArgs theDragEvents)
		{
			MultiPanePageDesigner aDsgn_Sel = GetSelectedPageDesigner();
			if (aDsgn_Sel != null)
				aDsgn_Sel.InternalOnDragEnter(theDragEvents);
		}

		protected override void OnDragLeave(EventArgs theArgs)
		{
			MultiPanePageDesigner aDsgn_Sel = GetSelectedPageDesigner();
			if (aDsgn_Sel != null)
				aDsgn_Sel.InternalOnDragLeave(theArgs);
		}

		protected override void OnDragOver(DragEventArgs theDragEvents)
		{
			MultiPaneControl aCtl = DesignedControl;
			Point pt = aCtl.PointToClient(new Point(theDragEvents.X, theDragEvents.Y));

			if (!aCtl.DisplayRectangle.Contains(pt))
				theDragEvents.Effect = DragDropEffects.None;
			else
			{
				MultiPanePageDesigner aDsgn_Sel = GetSelectedPageDesigner();
				if (aDsgn_Sel != null)
					aDsgn_Sel.InternalOnDragOver(theDragEvents);
			}
		}

		protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
		{
			MultiPanePageDesigner aDsgn_Sel = GetSelectedPageDesigner();
			if (aDsgn_Sel != null)
				aDsgn_Sel.InternalOnGiveFeedback(e);
		}
		#endregion

		#region Method everrides - Initialize/Dispose

		public override void Initialize(IComponent theComponent)
		{
			base.Initialize(theComponent);	// IMPORTANT! This must be the very first line

			// ISelectionService events
			ISelectionService aSrv_Sel = (ISelectionService)GetService(typeof(ISelectionService));
			if (aSrv_Sel != null)
				aSrv_Sel.SelectionChanged += new EventHandler(Handler_SelectionChanged);

			// IComponentChangeService events
			IComponentChangeService aSrv_CH = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (aSrv_CH != null)
			{
				aSrv_CH.ComponentRemoving += new ComponentEventHandler(Handler_ComponentRemoving);
				aSrv_CH.ComponentChanged += new ComponentChangedEventHandler(Handler_ComponentChanged);
			}

			// MultiPaneControl events
			DesignedControl.SelectedPageChanged += new EventHandler(Handler_SelectedPageChanged);

			// Prepare the verbs
			myAddVerb = new DesignerVerb("Add page", new EventHandler(Handler_AddPage));
			myRemoveVerb = new DesignerVerb("Remove page", new EventHandler(Handler_RemovePage));
			mySwitchVerb = new DesignerVerb("Switch pages...", new EventHandler(Handler_SwitchPage));

			myVerbs = new DesignerVerbCollection();
			myVerbs.AddRange(new DesignerVerb[] { myAddVerb, myRemoveVerb, mySwitchVerb });
		}

		protected override void Dispose(bool theDisposing)
		{
			if (theDisposing)
			{
				// ISelectionService events
				ISelectionService aSrv_Sel = (ISelectionService)GetService(typeof(ISelectionService));
				if (aSrv_Sel != null)
					aSrv_Sel.SelectionChanged -= new EventHandler(Handler_SelectionChanged);

				// IComponentChangeService events
				IComponentChangeService aSrv_CH = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				if (aSrv_CH != null)
				{
					aSrv_CH.ComponentRemoving -= new ComponentEventHandler(Handler_ComponentRemoving);
					aSrv_CH.ComponentChanged -= new ComponentChangedEventHandler(Handler_ComponentChanged);
				}

				// Control
				DesignedControl.SelectedPageChanged -= new EventHandler(Handler_SelectedPageChanged);
			}
			base.Dispose(theDisposing);
		}

		#endregion

		#region Method overrides - misc

		public override bool CanParent(Control theControl)
		{
			if (theControl is MultiPanePage)
				return !Control.Contains(theControl);
			else
				return false;
		}

		#endregion

		#region Propery overrides

		public override DesignerVerbCollection Verbs
			{ get { return myVerbs; } }

		#endregion

		#region Properties - own
		public MultiPaneControl DesignedControl
			{ get { return (MultiPaneControl)Control; } }

		public MultiPanePage DesignerSelectedPage
		{
			get { return mySelectedPage; }
			set
			{
				if (mySelectedPage != null)
					mySelectedPage.Visible = false;

				mySelectedPage = value;

				if (mySelectedPage != null)
					mySelectedPage.Visible = true;
			}
		}
		#endregion
	}

	/////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////

	internal class MultiPanePageDesigner : ScrollableControlDesigner
	{
		private Pen
			myBorderPen_Light,
			myBorderPen_Dark;


		private int  myOrigX = -1;               // store the original position of the
		private int  myOrigY = -1;               // mouse cursor

		private bool myMouseMovement = false;    // true if the mouse movement has been made


		#region Methods - ScrollableControlDesigner overrides
		protected override void Dispose(bool theDisposing)
		{
			if (theDisposing)
			{
				if (myBorderPen_Dark != null)
					myBorderPen_Dark.Dispose();

				if (myBorderPen_Light != null)
					myBorderPen_Light.Dispose();
			}

			base.Dispose(theDisposing);
		}

		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			DrawBorder(pe.Graphics);

			base.OnPaintAdornments(pe);
		}

		public override bool CanBeParentedTo(IDesigner theParentDesigner)
		{
			if (theParentDesigner != null)
				return (theParentDesigner.Component is MultiPaneControl);

			else
				return false;
		}

		protected override bool GetHitTest(Point pt)
		{
			return false;
		}

		protected override void OnMouseDragBegin(int theX, int theY)
		{
			myOrigX = theX;
			myOrigY = theY;

			// no call to base.OnMouseDragBegin
		}

		protected override void OnMouseDragMove(int theX, int theY)
		{
			if ( theX > myOrigX + 3 || theX < myOrigX - 3 ||
					 theY > myOrigY + 3 || theY < myOrigY - 3 )
			{
				myMouseMovement = true;

				base.OnMouseDragBegin(myOrigX, myOrigY);
				base.OnMouseDragMove(theX, theY);
			}
		}

		protected override void OnMouseDragEnd(bool theCancel)
		{
			bool aProcess = !myMouseMovement && Control.Parent != null;
			if (aProcess)
			{
				ISelectionService aSrv = (ISelectionService)GetService(typeof(ISelectionService));

				if (aSrv != null)
					aSrv.SetSelectedComponents(new Control[] { Control.Parent });
				else
					aProcess = false;
			}

			if (!aProcess)
				base.OnMouseDragEnd(theCancel);

			myMouseMovement = false;
		}
		#endregion

		#region Methods - own
		private Pen InternalEnsureDarkPenCreated()
		{
			if (myBorderPen_Dark == null)
				myBorderPen_Dark = InternalCreatePen(Color.Black);

			return myBorderPen_Dark;
		}

		private Pen InternalEnsureLightPenCreated()
		{
			if (myBorderPen_Light == null)
				myBorderPen_Light = InternalCreatePen(Color.White);

			return myBorderPen_Light;
		}

		private static Pen InternalCreatePen(Color theClr)
		{
			Pen aPen = new Pen(theClr);
			aPen.DashStyle = DashStyle.Dash;
			return aPen;
		}

		internal void InternalOnDragDrop(DragEventArgs theArgs)
		{
			OnDragDrop(theArgs);
		}

		internal void InternalOnDragEnter(DragEventArgs theArgs)
		{
			OnDragEnter(theArgs);
		}

		internal void InternalOnDragLeave(EventArgs theArgs)
		{
			OnDragLeave(theArgs);
		}

		internal void InternalOnGiveFeedback(GiveFeedbackEventArgs theArgs)
		{
			OnGiveFeedback(theArgs);
		}

		internal void InternalOnDragOver(DragEventArgs theArgs)
		{
			OnDragOver(theArgs);
		}

		protected void DrawBorder(Graphics theG)
		{
			MultiPanePage aCtl = DesignedControl;

			if (aCtl == null)
				return;
			else if (!aCtl.Visible)
				return;

			Rectangle aRct = aCtl.ClientRectangle;
			aRct.Width--;     // decrement the with so that the bottom and
			aRct.Height--;    // right lines could be visible

			theG.DrawRectangle(BorderPen, aRct);
		}

		private MultiPaneControlDesigner GetParentControlDesigner()
		{
			MultiPaneControlDesigner aDesigner = null;

			if (Control != null)
				if (Control.Parent != null)
				{
					IDesignerHost aSrv = (IDesignerHost)GetService(typeof(IDesignerHost));
					if (aSrv != null)
						aDesigner = (MultiPaneControlDesigner)aSrv.GetDesigner(Control.Parent);
				}

			return aDesigner;
		}
		#endregion

		#region Properties - ScrollableComponentDesigner overrides
		public override SelectionRules SelectionRules
		{
			get
			{
				SelectionRules aSelRules = base.SelectionRules;

				if (Control.Parent is MultiPaneControl)
					aSelRules &= ~SelectionRules.AllSizeable; // do not allow any handlers

				return aSelRules;
			}
		}

		public override DesignerVerbCollection Verbs
		{
			get
			{
				// 1. Obtain verbs from the base class
				DesignerVerbCollection aRet = new DesignerVerbCollection();

				foreach (DesignerVerb aIt in base.Verbs)
					aRet.Add(aIt);

				// 2. Obtain verbs from the parent control's designer
				MultiPaneControlDesigner aDs = GetParentControlDesigner();

				if (aDs != null)
					foreach (DesignerVerb aIt in aDs.Verbs)
						aRet.Add(aIt);

				return aRet;
			}
		}
		#endregion

		#region Properties - own
		protected MultiPanePage DesignedControl
		{
			get { return (MultiPanePage)Control; }
		}


		// Properties
		protected Pen BorderPen
		{
			get
			{
				if (Control.BackColor.GetBrightness() < 0.5)
					return InternalEnsureLightPenCreated();
				else
					return InternalEnsureDarkPenCreated();
			}
		}
		#endregion
	}

	/////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////

	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	[Serializable]
	public class MultiPaneControlToolboxItem : ToolboxItem
	{
		public MultiPaneControlToolboxItem()
			: base(typeof(MultiPaneControl))
		{
		}

		// Serialization constructor, required for deserialization
		public MultiPaneControlToolboxItem(SerializationInfo theInfo, StreamingContext theContext)
		{
			Deserialize(theInfo, theContext);
		}

		protected override IComponent[] CreateComponentsCore(IDesignerHost theHost)
		{
			return DesignerTransactionUtility.DoInTransaction
			(
				theHost,
				"MultiPaneControlTooblxItem_CreateControl",
				new TransactionAwareParammedMethod(CreateControlWithOnePage),
				null
			) as IComponent[];
		}


		#region Transaction Methods

		public object CreateControlWithOnePage(IDesignerHost theHost, object theParam)
		{
			// Control
			MultiPaneControl aCtl = (MultiPaneControl)theHost.CreateComponent(typeof(MultiPaneControl));

			// Page 1
			MultiPanePage aPg = (MultiPanePage)theHost.CreateComponent(typeof(MultiPanePage));

			aCtl.Controls.Add(aPg);

			return new IComponent[] { aCtl };
		}

		#endregion
	}


	internal class MultiPaneControlSelectedPageEditor : ObjectSelectorEditor
	{
		protected override void FillTreeWithData(Selector theSel,
			ITypeDescriptorContext theCtx, IServiceProvider theProvider)
		{
			base.FillTreeWithData(theSel, theCtx, theProvider);	//clear the selection

			MultiPaneControl aCtl = (MultiPaneControl) theCtx.Instance;

			foreach (MultiPanePage aIt in aCtl.Controls)
			{
				SelectorNode aNd = new SelectorNode(aIt.Name, aIt);

				theSel.Nodes.Add(aNd);

				if (aIt == aCtl.SelectedPage)
					theSel.SelectedNode = aNd;
			}
		}
	}
}
