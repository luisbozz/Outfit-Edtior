   M S B A M L   `   `   `  ���� I  EOutfit Editor, Version=2.61.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:xDevxDev  R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   WatermarkOld  �� 
TargetType �� Property 
 Nunito �� Value  	TextColor  TextBoxForegroundThemeBrush    xDev.CornerRadiusSetter �� 
TargetType � 	Resources   xDev.IsStringNotEmpty  Key ��Name  TextBoxBackground  TextBoxBorderInner
 � Tag ��Name    CornerRadius	 �� CornerRadius
 ��Name ��Name �� Triggers �� Value �� Binding �� Path �� RelativeSource �� 
TargetName ^� 
Conditions �� Binding  isStringNotEmpty �� 	Converter �� Value P� Property P� Value  TextBoxBorder 	 BorderGapMaskConverter J� SnapsToDevicePixels � ColumnDefinitions � RowDefinitions �� Padding J� OpacityMask `� 	Converter `� ConverterParameter �� ElementName�� D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     localclr-namespace:xDev  %�	  &	        7  7  7  7  &		 �    '�� �    7 	 �� "  ��.���  � ��# �  ��  � ��$
 12.0(���  � ��# �  ��  �@�Cursor$ IBeam����  �@��
CaretBrush# �  ��  � ��$ Center=���  �@  CornerRadius$ 2����  � �� �� " ��main�� ��  isStringNotEmpty .R�� .+��� $ border_header��#���  #���  $y�	Collapsed=�$��1q����� #��z $��11(�#���  $
��Bold"�#���  �� $ border_noheader��#���  #���  $��1q�#	 z $y�Visible=��� $
 PART_ContentHost��$��7,10,0,0q�#��z���� $ WaterMarkLabel��$��5,0,0,0q�#��z #��z��#��z��$y�	Collapsed=�
�������#���  {�.  #��z�� �� $  �� �� $
 Text� �� *Self+.a��� $ border_header�� � y�$ 	Collapsed=��� $ border_noheader�� � y�$ Visible=��� $ WaterMarkLabel�� � y�$ Visible=��� $ PART_ContentHost�� � ��$ 7,0,0,0q��� $ WaterMarkLabel�� � ��$ 5,0,0,0q�^�  ��  �� $
 Text� �� *Self+8   	 false��  �� $	 Tag� �� *Self+8  	 false.0��� $ border_header�� � y�$ Visible=��� $ border_noheader�� � y�$ 	Collapsed=��� $ WaterMarkLabel�� � y�$ 	Collapsed=�^�  ��  �� $
 Text� �� *Self+8  	 false��  �� $	 Tag� �� *Self+8   true.0��� $ border_header�� � y�$ 	Collapsed=��� $ border_noheader�� � y�$ Visible=��� $ WaterMarkLabel�� � y�$ 	Collapsed=��� $ PART_ContentHost�� � ��$ 7,0,0,0q��� $ WaterMarkLabel�� � ��$ 5,0,0,0q�P�  � {� .  . ���  � ��
 �iii�P�  �@J�	IsFocused . . ��� $ border_noheader��# �   � ���� $ border_header��# �   � ���� �� "  ��.���  � ��# �  ��  � ��$ 1q���  �@  CornerRadius$ 2����  � �� �� " ��.R��  .  �� $��6 ��� $
��Auto ��� $��* ��� $��6 � �� $
��Auto ��� $
��Auto ��� $��* ��� $��6 �.+��� #	 z $��1��$��3��$��0��$��4��#��z��
������ #��z���� $��Header��$ 3,1,3,0q�$��0��$��2��$��1��._��� $��Header����. # z �� $��2��$��1��$��2��#��z��# z �� $��1��$��3��$��4��#��z��#��z��#	 z  `� 8   $ 7��.1��� $ Header��$ ActualWidth��� $ ActualWidth� �� *Self+�� $ ActualHeight� �� *Self+