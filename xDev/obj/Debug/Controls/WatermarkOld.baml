   M S B A M L   `   `   `  ����?  ;xDev, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:xDevxDev  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   WatermarkOld  �� 
TargetType �� Property 
 Nunito �� Value  	TextColor  TextBoxForegroundThemeBrush    xDev.CornerRadiusSetter �� 
TargetType � 	Resources   xDev.IsStringNotEmpty  Key ��Name  TextBoxBackground  TextBoxBorderInner
 � Tag ��Name    CornerRadius	 �� CornerRadius
 ��Name ��Name �� Triggers �� Value �� Binding �� Path �� RelativeSource �� 
TargetName ^� 
Conditions �� Binding  isStringNotEmpty �� 	Converter �� Value P� Property P� Value  TextBoxBorder 	 BorderGapMaskConverter J� SnapsToDevicePixels � ColumnDefinitions � RowDefinitions �� Padding J� OpacityMask `� 	Converter `� ConverterParameter �� ElementName�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      localclr-namespace:xDev  5      %S  &	        7  7  7  7  &		 g
    '�� z
    7 	 �� 5      "  ��6!   .�5   
   ��  � ��# �  6'   �� 5   
    � ��$
 12.0(�6%   �� 5   
    � ��# �  6'   �� 5	   
    �@�Cursor$ IBeam��6#   �� 5
   
    �@��
CaretBrush# �  6'   �� 5   
    � ��$ Center=�65   �� 5   
    �@  CornerRadius$ 2��6B   �� 5   
    � �� 5      �� 5      " ��6"   main�� 5      ��  5      isStringNotEmpty 61   5      .R�5      � .+�5      �� $ border_header��6#   #���  6:   #���  6k   $y�	Collapsed=�6�   $��1q�6�   ��5      �� 5   "   #��z 6,   $��11(�6I   #���  6W   $
��Bold"�6}   #���  6�   5      �� 5      $ border_noheader��6!   #���  6:   #���  6k   $��1q�6�   #	 z 6�   $y�Visible=�6�   �� 5      $
 PART_ContentHost��69   $��7,10,0,0q�6'   #��z��6S   �� 5      $ WaterMarkLabel��61   $��5,0,0,0q�6    #��z 6I   #��z��5       #��z��6_   $y�	Collapsed=�5       
�������#���  6I   {�.  #��z��6�   5       5      �� 5$      $  ��6j    6&   �� $
 Text� �� *Self+.a�5%      �� $ border_header��6%    � y�$ 	Collapsed=�6V   �� 5&      $ border_noheader��6%    � y�$ Visible=�6X   �� 5'      $ WaterMarkLabel��6%    � y�$ Visible=�6W   �� 5(      $ PART_ContentHost��6%    � ��$ 7,0,0,0q�6U   �� 5)      $ WaterMarkLabel��6%    � ��$ 5,0,0,0q�6S   5*      ^� 5.       5/      �� 50   "    6,   �� $
 Text� �� *Self+8   	 false6�   �� 51   "    6,   �� $	 Tag� �� *Self+8  	 false6�   .0�53      �� $ border_header��6%    � y�$ Visible=�6V   �� 54      $ border_noheader��6%    � y�$ 	Collapsed=�6X   �� 55      $ WaterMarkLabel��6%    � y�$ 	Collapsed=�6W   56      ^� 59       5:      �� 5;   "    6,   �� $
 Text� �� *Self+8  	 false6�   �� 5<   "    6,   �� $	 Tag� �� *Self+8   true6�   .0�5>      �� $ border_header��6%    � y�$ 	Collapsed=�6V   �� 5?      $ border_noheader��6%    � y�$ Visible=�6X   �� 5@      $ WaterMarkLabel��6%    � y�$ 	Collapsed=�6W   �� 5A      $ PART_ContentHost��6%    � ��$ 7,0,0,0q�6U   �� 5B      $ WaterMarkLabel��6%    � ��$ 5,0,0,0q�6S   5C      P� 5F       � {� .  . �5G      ��  � ��
 �iii�6;   5H      P� 5I       �@J�	IsFocused . . �5J      �� $ border_noheader��6%   # �  6Y    � ���� 5K      $ border_header��6%   # �  6W    � ��5L      5N      5P      5Q      �� 5S      6   �� 5T      "  ��6   .�5U   
   ��  � ��# �  6(   �� 5V   
    � ��$ 1q�6,   �� 5W   
    �@  CornerRadius$ 2��6B   �� 5X   
    � �� 5Y      �� 5Z      " ��6"   .R�5[      �  .  5\      �� 5]      $��6 �6/   �� 5^      $
��Auto �6/   �� 5_      $��* �6/   �� 5`      $��6 �6/    5b      �� 5c      $
��Auto �6,   �� 5d      $
��Auto �6,   �� 5e      $��* �6,   �� 5f      $��6 �6,   .+�5h      �� #	 z 6!   $��1��6h   $��3��6u   $��0��6�   $��4��6�   #��z��6�   
������ #��z��6�   �� 5i      $��Header��6!   $ 3,1,3,0q�6/   $��0��6A   $��2��6N   $��1��6_   ._�5j      �� $��Header��6/   ��. # z 6a   5k      �� 5l      $��2��6+   $��1��68   $��2��6H   #��z��6\   # z 6   �� 5m      $��1��6!   $��3��6.   $��4��6?   #��z��6T   #��z��6�   #	 z 6�    5n      `� 5o   "   8   $ 7��6c   .1�5p   &   �� $ Header��6.   $ ActualWidth�6C   �� 5q   &   $ ActualWidth�6.    6A   �� *Self+�� 5r   &   $ ActualHeight�6.    6B   �� *Self+5s   #   5u      5v      5w      5y      5z      5{      