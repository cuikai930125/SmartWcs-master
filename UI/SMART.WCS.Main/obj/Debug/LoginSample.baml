   M S B A M L   `   `   `  ����  	SMART.WCS     SMART.WCS.Main.LoginSample   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35a ]DevExpress.Xpf.Core.v19.1, Version=19.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49aR NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   ?dxe5http://schemas.devexpress.com/winfx/2008/xaml/editors  5   	   :d2http://schemas.microsoft.com/expression/blend/2008  5   	   ;dx2http://schemas.devexpress.com/winfx/2008/xaml/core  5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &   DevExpress.Xpf.Core.ThemeManager    	ThemeName0  *DevExpress.Xpf.Core.ThemeNameTypeConverter$  Office2019Colorful 5	   	    � WindowStartupLocation$ CenterScreen=�5
   	    � WindowState$ 	Maximized=�5   	   ���	#FFF9F9F9$
��1024��5   	   $	��768��6   $
��1024��5   	   $	��768��6    � Title$ Login��5   	    � 	Resources 5       �� 
TargetType   LoginButtonStyle �� Property �� Value �� RelativeSource��%�  &	        �� 5   
   " ��6   .�5      ��  �@J�	AllowDrop .  6*   �� 5       � ��$ Center=�6;   �� 5       � ��$ Center=�69   �� 5       � ��$ 0q�6(   �� 5       �@��	ClickMode$ Press=�6*   �� 5       � ��$ Stretch=�6;   �� 5       � ��$ Stretch=�69   �� 5       � ��$ 3,0,0,0q�6'   �� 5       � ��$
 Auto��6(   �� 5       �@�Cursor$
 Hand��6)   �� 5       � �� 5      �� 5      .R�5       � .+�5!      �� $��0q�6%   ���#31383E._�5"   "   �� 
�������$��Center=�6�   $��Center=�6�   ��6?   �� *	Content+ �� *TemplatedParent+6�   5#      5$      5%      5'      5(      5)      .��5+      � 	 � RowDefinitions	 5-   
   �� 5.      $��20 �6   �� 5/      $��30 �6   �� 50      $��5 �6   �� 51      $	��150 �6   �� 52      $	��130 �6   �� 53      $��1 �6   �� 54      $��40 �6   �� 55      $��10 �6   �� 56      $��40 �6   �� 57      $��10 �6   �� 58      $��30 �6   �� 59      $��20 �6   �� 5:      $��48 �6   �� 5;      $��20 �6   �� 5<      $��30 �6   �� 5=      $��20 �6   �� 5>      $��60 �6   �� 5?      $��* �6   �� 5@      $��40 �6   
 � ColumnDefinitions
 5C   
   �� 5D      $��20 �6!   �� 5E      $	��110 �6!   �� 5F      $��* �6!   �� 5G      $	��180 �6!   �� 5H      $	��180 �6!   �� 5I      $��* �6!   �� 5J      $	��110 �6!   �� 5K      $��20 �6!   .+�5N   
   )  #DevExpress.Xpf.Editors.ComboBoxEdit  -   5          Name$ cboLang��5N      $��1��6/   $��6��6?   %  DevExpress.Xpf.Editors.BaseEdit   NullText$ SELECT��5O      +  %DevExpress.Xpf.Editors.LookUpEditBase   ValueMember$
 CODE��61      DisplayMember$
 NAME��6E   �� 5Q   
   -   5         ��Name$ lgLogo��5Q      $��18��6#   $��1��63   .+�5R      �� -   5        $��Img_1��5R   `   $
��Fill=�6   $9��3/SMART.WCS.Resource;component/Image/lgcns_small.png��6#   6`   5S      �� 5V   
   -   5        $��Img_2��5V   b   $
��Fill=�6   $?��9/SMART.WCS.Resource;component/Image/logo_autumn_login.png��6   $��4��6o   $��3��6~   $��2��6�   %  DevExpress.Xpf.Editors.TextEdit  5Y   
   -   5          Name$ 	txtUserID��5Y      $��6��61   $��3��6A   $��2��6Q   $ User Id��6e     5[   
   -   5        $ txtPwd��5[      $��8��6.   $��3��6>   $��2��6N   $ Password��6b     5]   
   -   5        $ 	cboCenter��5]      $��10��61   $��3��6B   $��2��6R   $ SELECT��5^      $
 CODE��61   $
 NAME��6E   &   DevExpress.Xpf.Editors.CheckEdit  5b   
   -   5          Name$ chkRememberID��5b      $��14��61   $��3��6B      Content)  #DevExpress.Xpf.Core.ObjectConverter$ Remember ID 5c      $
��Left=�61   (	  "DevExpress.Xpf.Editors.ListBoxEdit	  5e   
   -	   5         	 Name$ lbRadio��5e      $��14��6-   $��4��6<    	  ValueMember$ Content��6P    	  SelectedIndex$ 0��6h   $��Center=�6}   $��Center=�6�   $��0q�6�      
ShowBorder .  $y�Visible=�6�    	  StyleSettings 5f      :
  4DevExpress.Xpf.Editors.RadioListBoxEditStyleSettings
  5g       	  
ItemsPanel 5i      �� 5j      .R�5k      ��  �� Orientation$ 
Horizontal=�6!   5l       	  Items 5n      ,  &DevExpress.Xpf.Editors.ListBoxEditItem  5o      -
   5          Name$ rdoREAL��5o   )   $��0q�6=   $��0q�6K   $��운영��6Y   $��Center=�6i   $��Center=�6�     5p      -   5        $ rdoDEV��5p   )   $��0q�6=   $��0q�6K   $��개발��6Y   $��Center=�6i   $��Center=�6�   5r      �� 5~   
   -   5         ��Name$ btnLogin��5~      $��	로그인��6%   $��12��64   $��3��6C   $��2��6S   #��[  5      $��20(�5�   N   5�      5�      