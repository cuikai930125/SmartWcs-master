   M S B A M L   `   `   `  ����  	SMART.WCS    SMART.WCS.Main.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35a ]DevExpress.Xpf.Core.v19.1, Version=19.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aR NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   ;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  5   	   ?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors  5   	   a ]DevExpress.Xpf.Grid.v19.1, Version=19.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49af bDevExpress.Xpf.Grid.v19.1.Core, Version=19.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a>dxg2http://schemas.devexpress.com/winfx/2008/xaml/grid   5   	   j fDevExpress.Xpf.LayoutControl.v19.1, Version=19.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aEdxl;http://schemas.devexpress.com/winfx/2008/xaml/layoutcontrol  5   	   :d2http://schemas.microsoft.com/expression/blend/2008  5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5	   	   -   5          �Name$  frmMainWindow��5   	   $��1q�5   	   &   DevExpress.Xpf.Core.ThemeManager   	ThemeName0  *DevExpress.Xpf.Core.ThemeNameTypeConverter$ Office2019Colorful 5   	    � WindowStartupLocation$ CenterScreen=�5   	    � WindowState$ 	Maximized=�5   	    � UseLayoutRounding .  J� SnapsToDevicePixels .  � Title$ 	SMART WCS��5   	   $
��1024��5   	   $	��768��6   $
��1024��5   	   $	��768��6   ��5   	   Z�  � 	Resources 5         
BtnBkStyle �� 
TargetType	 �� Property
 �� Value  ImageMouseOverStyle �� Triggers P� Property P� Value  maximizeImg � Cursor J� Opacity  
restoreImg  TransparentStyle �� 
TargetType ��Name �� Triggers �� 
TargetName��%�  &	        &	 l     &	 �     &	 l    &	 �    �� 5I   
   " ��6#   .�5J      �� 	 � ��
 6+   Z� �� 5K      	 � ��
 6,   Z� 5L      �� 5N   
   " ��6,    5O      P� 5P       �@J�IsMouseOver . . �5Q      �� 	 �@J�Opacity$	
 0.9Q�60   5R      5T      �� 5W   
   $=��7/SMART.WCS.Resource;component/Image/Btn_hd_maximize.png��6$   $
 Hand��6e   $	 0.7Q�6s   �� 5X   
   $<��6/SMART.WCS.Resource;component/Image/Btn_hd_restore.png��6#   $
 Hand��6c   $	 0.7Q�6q   �� 5\   
   " ��6)   .�5]      �� 	 � ��
 5^      �� 5_      " ��6&   .R�5`      �� $ templateBorder��6!   
������ ._�5a      � 
������ .+�5b   "   �� 65   5c      5d       5e      P� 5f       � �� . . �5g   "   �� $ templateBorder��6)   	 � ��
 �#5050506[   5h      P� 5i       �@J�IsMouseOver . . �5j   "   �� $ templateBorder��6)   	 � ��
 �#5050506[   5k      5m      5o      5p      5s      .��5v      �� $��0q�6   ���#ffffff$��0q�5w   "    � ContextMenu 5y   
   �� 5z      .@�5{      m� -   5         m�Name$ menuItemTabAllClose��5{      5|       J� Effect 5   
   3  -System.Windows.Media.Effects.DropShadowEffect  5�         Color$ #505050��6       ShadowDepth$ 0Q�60      Opacity$	 0.9Q�6@      
BlurRadius$ 10Q�6N      	Direction$ 0Q�6^   ._�5�   
   ]� -   5        $��
dockPanel1��5�      .+�5V     �� $	��Top=�6   $��0q�6-   $��50��6E   ���#F5F5F5._�5W     �  � ColumnDefinitions 5X     �� 5Y     $	��150 �6-   �� 5Z     $��* �6-   �� 5[     $	��300 �6-   .+�5^     � $��0��6   $��10,0,0,0q�61   .+�5b     �� -   5         ��Name$ imgLogo��5b  !   $=��7/SMART.WCS.Resource;component/Image/CoupangMainLogo.png��63   $	��100��5c  !   $
��Left=�6-   $��Center=�6I   5d     � 5f     -   5         �Name$ topGridMenuArea��5f     $��1��69   � 5t     $��2��6   $��Right=�61    5u     �� 5v     $
��Auto �61   �� 5w     $	��100 �61   �� 5z     $
��Auto �61   .+�5�     �� $4��./SMART.WCS.Resource;component/Image/Idicon.png��6!   $��25��6]   � 5�     $��1��6!   $��0,0,10,0q�65    � RowDefinitions 5�     �� 5�  "   $��6* �61   �� 5�  "   $��4* �61   .+�5�     �� -   5          ��Name$  lblUserName��5�  *   $��0��5�  )   $��Center=�5�  *   $��Center=�5�  *   ���#888888$
��Bold"�5�  )   $
��15px(�5�  *   $��0,5,0,0q�5�  )   ! �� Padding$! 0q�5�  )   �� 5�     -   5        $  lblCenterName��5�  )   $��0��5�  )   $��1��6=   $��Center=�5�  )   $��Center=�5�  )   ���#8D8D8D$
��11px(�5�  )   $��0,0,0,5q�5�  )   $! 0q�5�  )   5�     � 5�     -   5        $ gridAreaNotice��5�  !   $��2��69   $��0,0,10,0q�6M   ��6a   Z� .+�5�     �� -	   5        $ 	imgNotice��5�  %   $8��2/SMART.WCS.Resource;component/Image/alarm_Icon.png��69   $��22��5�  )   $��22��65   � 5�     -
   5        $ gridAreaNoticeCount��5�  %   .+�5�  "   A� -   5        " A�Name$" 	eliNotice��5�  -   $��20��6B   $��20��6N   
���  ��z�5�  &   T� 5�  *   # T� X$	# -10Q�6=   $ T� Y$$ -8Q�6F   5�  #   �� 5�  "   -   5        $  	lblNotice��5�  -   $
��12px(�6A   $�� ��6Q   $
��Bold"�6\   $��
-17,3,0,20q�5�  -   $
 Hand��6A   5�     5�     5�     5�     5�     �� 5�     $��Bottom=�6   ���#FFFFFF$��0,1,0 1q�5�     ���#ced0d4$��34��5�     ._�5�     �  5�     �� 5�     $��12 �6-   �� 5�     $
��Auto �6-   �� 5�     $��* �6-   �� 5�     $��40 �6-   �� 5�     $
��Auto �6-   �� 5�     $��10 �6-    5�     �� 5�     $��1 �6)   �� 5�     $��33 �6)   .+�5�     �� -   5        $  txtMsg��5�  !   $��1��5�  !   $��1��5�  !   $�� ��5�  !   $
��Left=�5�  !   $��Center=�5�  !   $
��14px(�5�  !   $
��Bold"�5�  !   ���#8888885�  !   �� 5�     $��1��6!   $��2��5�  !   $��Right=�5�  !   % �� TextAlignment$% Right=�5�  !   $
��12px(�5   !   ���#888888$.��(Copyright ⓒ LG CNS all rights reserved��5  !   $	��880��5  !   �� 5     -   5        $  lblClock��5  !   $��1��5  !   $��4��5  !   $��2018-04-26 PM 02:30:46��5	  !   $��Center=�5
  !   $��Right=�5  !   $
��12px(�5  !   ���#8888885  !   5D     5E     � 5G     $��Disabled=�6   $��Disabled=�6J   .+�5H     0  *DevExpress.Xpf.LayoutControl.LayoutControl  -   5        &  Name$& layout��5H  %   .  (DevExpress.Xpf.LayoutControl.LayoutGroup'   Orientation$' 
Horizontal=�5I  %   $	��Top=�5J  %   4  .DevExpress.Xpf.LayoutControl.LayoutControlBase(   Padding$( 0q�5K  %   ��61   �� *ActualHeight+) �� Mode$) OneWay=�* �� RelativeSource* �� *FindAncestor++ �� AncestorType"+ �.+�5N       -   5        $��layoutGrpTree��5N  )   $	��199��5O  )   $��Stretch=�5P  )   $
��Left=�5Q  )   $	��200��6D   ,   AllowHorizontalSizing, . .+�5S     � -   5        $
 grid��5S     $��Stretch=�6-    5T     �� 5U  "   $��8 �61   �� 5V  "   $��20 �61   �� 5W  "   $��5 �61   �� 5X  "   $��8 �61   �� 5Y  "   $��* �61   .+�5\     � $��1��6%   $��10,0,0,0q�5]  %   .+�5_  "   &   DevExpress.Xpf.Core.SimpleButton  -   5        -  Name$- BtnMenuAllShow��5b  5   $��0q�5_  5   $��0q�5`  5   $
��Left=�5a  5   .��5d  &   �� $=��7/SMART.WCS.Resource;component/Image/ico_lt_tree_exp.png��6-   5e  #     5i  "   -   5        $- BtnMenuSelectedFocus��5l  5   $��20,0,0,0q�5i  5   $��0q�5j  5   $
��Left=�5k  5   .��5n  &   �� $A��;/SMART.WCS.Resource;component/Image/ico_lt_tree_current.png��6-   5o  #     5s  "   -   5        $- BtnMenuAllHide��5v  5   $��40,0,0,0q�5s  5   $��0q�5t  5   $
��Left=�5u  5   .��5x  &   �� $?��9/SMART.WCS.Resource;component/Image/ico_lt_tree_close.png��6-   5y  #   �� 5~  "   -   5        . ��Name$. BtnMenuFold��5�  5   $��4��5~  )   $��0q�5  5   $��0q�5�  5   #��[  5�  5   $��Right=�5�  5   .��5�  &   �� -   5        $ imgFold��5�  -   $<��6/SMART.WCS.Resource;component/Image/Btn_lt_winfold.png��5�  -   5�  -   5�  #   �� 5�  "   -   5        $. 
BtnMenuExp��5�  5   $��1��5�  ,   $��4��5�  5   $��0q�5�  5   $��0q�5�  5   #��[  5�  5   $��Right=�5�  5   .��5�  &   �� -   5        $ imgExp��5�  -   $;��5/SMART.WCS.Resource;component/Image/Btn_lt_winexp.png��5�  -   5�  -   5�  #   5�     � 5�     -   5        $ gridTreeControl��5�  0   $��4��6#   .+�5�  "   )  #DevExpress.Xpf.Grid.TreeListControl  -   5        $��treeListControl��5�  9   $	��Top=�5�  9   )	  #DevExpress.Xpf.Grid.DataControlBase/ 	  
ShowBorder/ .  $��0q�5�  9   ��5�  9   �� *ActualHeight+0 �� ElementName$0 gridTreeControl��$) OneWay=�1 	  ItemsSource1 5�  9   �� *
MenuList+2   Columns2 5�  &   (
  "DevExpress.Xpf.Grid.TreeListColumn
  5�  *   $  DevExpress.Xpf.Grid.ColumnBase3   	FieldName$3 MENU_NM��6=   3  -DevExpress.Xpf.Core.DXFrameworkContentElement4   	IsEnabled4 .  $  DevExpress.Xpf.Grid.BaseColumn5   Width2  ,DevExpress.Xpf.Grid.GridColumnWidthConverter$	5 160 5�  =   6   CellTemplate6 5�  .   �� 5�  2   .R�5�  6   ]� .+�5�  :   %  DevExpress.Xpf.Editors.TextEdit  7  Name$7 PART_Editor��6G   %  DevExpress.Xpf.Editors.BaseEdit8   
ShowBorder8 .  6\   &   DevExpress.Xpf.Editors.ImageEdit  5�  :   9  Name$9 imgApp��6H   $
��Left=�6X   $
��Left=�6s   $y�	Collapsed=�5�  H   $��Center=�6_   $��15��5�  H   $��15��6S   $��0,1,0,0q�6_   8 .  :   
IsReadOnly: . 6�   5�  7   5�  3   5�  +   
  5�  *   $3 MENU_ID��6=   ;   Visible; .  $	5 160 5�  9   
  5�  *   $3 	PARENT_ID��6=   ; .  6S   
  5�  *   $3 MENU_URL��6=   ; .  6R   <   View< 5�  &   &   DevExpress.Xpf.Grid.TreeListView  5�  *   -   5        $��treeListView��5�  =   =   KeyFieldName$= TREE_ID��5�  =   >   ParentFieldName$> 	PARENT_ID��5�  =   ?   IndicatorWidth$? 0Q�5�  =   $
 Hand��5�  =   &   DevExpress.Xpf.Grid.DataViewBase@   AllowEditing@ .  -  'DevExpress.Xpf.Core.ScrollBarExtensionsA   ScrollBarMode$A TouchOverlap=�5�  =   B   AutoExpandAllNodesB . C   RowStyleC 5�  .   �� 5�  2   $  DevExpress.Xpf.Grid.RowControl"  68    5�  6   �� 5�  :   D �� BindingD 6F   �� *Row.MENU_URL+E �� ValueE  6g   .a�5�  >   �� 	 � ��$

 Bold"�6[   5�  ;   5�  3   5�  +   5�  #   5�     5�     5�     �� 5�     ���#ced0d4$��1,0,0,0q�63   ._�5�       -   5        $��layoutGrpTab��5�  -   , . $��Stretch=�5�  )   $��Stretch=�5�  )   $
��Auto��5�  )   ��5�  A   �� *ActualHeight+$) OneWay=�* �� *FindAncestor+"+ �.+�5�     � ��6#   �� *ActualHeight+$) OneWay=�* �� *FindAncestor+"+ �.+�5�  "   &   DevExpress.Xpf.Core.DXTabControl  -   5        F  Name$F 
tabControl��5�  5   $��0q�6b   $	��Top=�5�  1   G   TabContentCacheMode$G CacheAllTabs=�5  1   H   AllowMergingH 5�  J   Z� ��5�  I   �� *ActualHeight+$) OneWay=�* �� *FindAncestor+"+ ���5   1   �� *Items+I   ViewI 5  &   .  (DevExpress.Xpf.Core.TabControlScrollView  5  *   5	  +   5  #   � 5  "   -   5        $ imgMain��5  '   $y�	Collapsed=�68    5  &   �� 5  *   $	��330 �68   �� 5  *   $��* �68   �� 5  *   $	��330 �68   .+�5  &   �� $
��Left=�6,   $��0��6G   J K� ZIndex$J 1��6T   $	��Top=�6e   $��0q�6}   $
��Auto��6�   $C��=/SMART.WCS.Resource;component/Image/img_mainlist_left_top.png��5  ,   �� 5  &   $
��Fill=�6,   $��0��6;   $��3��6H   $<��6/SMART.WCS.Resource;component/Image/bg_mainlist_yg.png��6Y   $J 0��6�   �� 5  &   $��Right=�6,   $J 1��6H   $��2��6Y   $��Bottom=�6f   $��0q�6�   $F��@/SMART.WCS.Resource;component/Image/img_mainlist_rightbottom.png��5  )   5  #   5     5     5      5!     5"     5$     5%     5&     