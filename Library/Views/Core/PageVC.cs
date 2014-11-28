//
// mTouch-PDFReader library
// PageVC.cs
//
//  Author:
//       Alexander Matsibarov <amatsibarov@gmail.com>
//
//  Copyright (c) 2014 Alexander Matsibarov
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using mTouchPDFReader.Library.Data.Enums;

namespace mTouchPDFReader.Library.Views.Core
{
	public class PageVC : UIViewController
	{
		#region Data
		private readonly RectangleF _viewFrame;
		private readonly AutoScaleModes _autoScaleMode;
		public int PageNumber { get; private set; }

		public PageView PageView {
			get {
				return View as PageView;
			}
		}

		public bool IsNotEmptyPage {
			get {
				return PageView != null;
			}
		}
		#endregion

		#region Logic
		public PageVC(IntPtr handle) : base(handle)
		{
		}
		
		[Export("initWithCoder:")]
		public PageVC(NSCoder coder) : base(coder)
		{
		}
		
		public PageVC(RectangleF viewFrame, AutoScaleModes autoScaleMode, int pageNumber) : base(null, null)
		{
			_viewFrame = viewFrame;
			_autoScaleMode = autoScaleMode;
			PageNumber = pageNumber;
		}	

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (PageNumber == -1) {
				View.BackgroundColor = UIColor.Clear;
			} else {
				View = new PageView(_viewFrame, _autoScaleMode, PageNumber);
			}
		}
		#endregion
	}
}
