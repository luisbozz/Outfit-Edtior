   M S B A M L   `   `   `  ���� I  EOutfit Editor, Version=2.61.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:xDevxDev      xDev.OutfitEditor   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     $localclr-namespace:xDev     -     � Title$  Outfit Editor��$��463.668��$��838.867��$	��345��$	��515�� � WindowStyle$
 None=����#202225
������� 
  Nunito#���    � 	Resources  �� 
TargetType �� Property �� Value  	TextColor �� Triggers P� Property P� Value  clothing	 �� Orientation
 � ToolTip ��Name��%�  '��       '�� O     '�� �     '��     &	 Q    �� " ��.���  � ��# �   ��  � ��# �  ��  � ��$ Center=��� " ��.���  � ��# �   ��  � ��# �  �� " ��.���  � ��# �   ��  � ��$ 16(���  � ��# �   P�  � {� .  . ���  � ��
 ������� " ��.���  � ��# �   ��  � ��# �  ��  � ��$ Center=��� .R��� $	 
Horizontal=�.+��� ���� *DisplayValue+
 �� $ imgmask��'  !System.Windows.Shell.WindowChrome   WindowChrome      GlassFrameThickness$ -1q�   ResizeBorderThickness$ 5q�   CaptionHeight$ 0Q�   UseAeroCaptionButtons . .��� ���#202225 � RowDefinitions �� $
��auto ��� $��* �.+�� -   $��0�����#2F3136$��30�� � ColumnDefinitions �� $��60 ��� $��* ��� $��30 ��� $��30 �.+��� $��xDev | lUiSBOzZ_RSG��
�������$��1��$��Center=�$��Center=��� -    ��Name$ BtnMinimize��$��2��  TitleBarButton#���     IsHitTestVisibleInChrome . .��F� -    F�Name$ minimizepath��"����        р�[ ��[ � �     	 �� Stretch$ Uniform=�
�������$��3q��� -   $ BtnClose��$��3��#���   . .��F� -   $ 	closepath��+����        �@B @B �    @B �@B     	$ Uniform=�
�������$��3q��� -    ��Name$ 	MainPages��
������ $��0q�$��1��.@��� -   $��MainPage��$��MainPage��.��� $��5q� �� $	��100 ��� $��* �.+��� $��0��  SectionBackgroundBrush#��[  �� CornerRadius$ 5�� �� Padding$ 5q�$��
15,15,15,5q�$��Stretch=�$��Stretch=� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     
BlurRadius$ 15Q�   	Direction$	 -90Q�   RenderingBias$ Quality=�   ShadowDepth$ 2Q�._�� $��50�� �� $
��0.3* ��� $
��0.4* ��� $
��0.3* �.+�� $��0��$	��5 0q�$��30�� �� $
��0.5* ��� $
��0.5* � �� $
��0.5* ��� $
��0.5* �.+��� $��Status��$��Center=�$��0��$��0���� $��ID��$��Center=�$��1��$��0���� -	    ��Name$ 	Lblattach��$��Center=�$��0��$��1���� -
   $ Lblpid��$��Center=�$��1��$��1���� $��1��
������ $	��5 0q�$��30��._��� -     ��Name$  ddoutfitnumber��#���   
! � Tag$! Outfits��" �� DisplayMemberPath$
" Name���� -   # ��Name$# tbspoof��$��2��  	Watermark#��[ $
 Name Spoofer��$��30��$	��5 0q�� -   $ �Name$$ MainGrid��$��1��.+��� -   % ��Name$
% Main��#��[ $ 5��$ 5q�$��
15,5,15,15q�$��Stretch=�$��Stretch=�   $ 15Q�$	 -90Q�$ Quality=�$ 2Q�._��  �� $��40 ��� $��* �.+�� $��Center=�$��Center=�$��0�� �� $
��auto ��� $
��auto �.+��� -   $# tbname��$��0��#��[ $! Outfit Name��$��30��$	��300��$	��0 5q��� -   $ ImageNameHelp��$��1�� 
 help32#���  $��30��$��5,5,0,5q��� $��	Easy Mode��$��Center=�$��0,0,40,0q�$��Right=��� -   & ��Name$& cbadv��$��27��$��27��$��0,0,5,0q�$��Right=��� $��1��$��Disabled=�$
��Auto=�$	��5 0q�.���  �� $
��auto ��� $
��auto �.+�� $��0�� �� $��50 ��� $��* �.+��� $��0��$��0��$��2��$��Center=�#���  $��28(�$��
Components��$
��Bold"�� ' � Orientation$' Vertical=�$��1��.+�� $��2q�y��� ( �� ElementName$( tbtop��) �� Path$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -   $# tbtop��$��0��#��[ $	! Top��$	��210��$��30��$	��5 0q��� -   $# tbtoptexture��$��1��#��[ $��30��$��30��� $��2q�y��� $( tbtop2��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -   $# tbtop2��$��0��#��[ $! 
Undershirt��$	��210��$��30��$	��5 0q��� -   $# tbtop2texture��$��1��#��[ $��30��$��30��� $��2q�y��� $( tblegs��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -   $# tblegs��$��0��#��[ $
! Legs��$	��210��$��30��$	��5 0q��� -   $# tblegstexture��$��1��#��[ $��30��$��30��� $��2q�y��� $( tbfeet��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -   $# tbfeet��$��0��#��[ $
! Feet��$	��210��$��30��$	��5 0q��� -   $# tbfeettexture��$��1��#��[ $��30��$��30��� $��2q�y��� $( tbmisc��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -   $# tbmisc��$��0��#��[ $! Accessories��$	��210��$��30��$	��5 0q��� -   $# tbmisctexture��$��1��#��[ $��30��$��30��� $��2q�y��� $( tbpara��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -   $# tbpara��$��0��#��[ $
! Bags��$	��210��$��30��$	��5 0q��� -   $# tbparatexture��$��1��#��[ $��30��$��30��� $��2q�y��� $( tbmask��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -   $# tbmask��$��0��#��[ $
! Mask��$	��210��$��30��$	��5 0q��� -   $# tbmasktexture��$��1��#��[ $��30��$��30���� -    $  ddtops��$y�	Collapsed=�$��2q�$��30��$	��250��$
! Tops��$

 Tops��#��[ �� -!   $  ddundershirts��$y�	Collapsed=�$��2q�$��30��$	��250��$! Untershirts��$
 Untershirts��#��[ �� -"   $  ddlegs��$y�	Collapsed=�$��2q�$��30��$	��250��$
! Legs��$

 Legs��#��[ �� -#   $  ddshoes��$y�	Collapsed=�$��2q�$��30��$	��250��$! Shoes��$
 Shoes��#��[ �� -$   $  ddaccessories��$y�	Collapsed=�$��2q�$��30��$	��250��$! Accessories��$
 Accessories��#��[ �� -%   $  ddtorsos��$y�	Collapsed=�$��2q�$��30��$	��250��$! Torsos��$
 Torsos��#��[ �� -&   $  ddmask��$y�	Collapsed=�$��2q�$��30��$	��250��$
! Mask��$

 Mask��#��[ �� -'   $  ddhair��$y�	Collapsed=�$��2q�$��30��$	��250��$
! Hair��$

 Hair��#��[ � $��2q�y��� $( tbgloves��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -(   $# tbgloves��$��0��#��[ $! Gloves��$	��210��$��30��$	��5 0q��� -)   $# tbglovestexture��$��1��#��[ $��30��$��30��� $��2q�y��� $( tbdecals��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -*   $# tbdecals��$��0��#��[ $! Decals��$	��210��$��30��$	��5 0q��� -+   $# tbdecalstexture��$��1��#��[ $��30��$��30��� $��2q�y��� $( tbarmor��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -,   $# tbarmor��$��0��#��[ $! Armor��$	��210��$��30��$	��5 0q��� --   $# tbarmortexture��$��1��#��[ $��30��$��30��� $��1�� �� $��50 ��� $��* �.+��� $��0��$��0��$��2��$��Center=�#���  $��28(�$��Props��$
��Bold"�� $' Vertical=�$��1��.+�� $��2q�y��� $( tbhat��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -.   $# tbhat��$��0��#��[ $
! Hats��$	��210��$��30��$	��5 0q��� -/   $# tbhattexture��$��1��#��[ $��30��$��30��� $��2q�y��� $( 	tbglasses��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -0   $# 	tbglasses��$��0��#��[ $! Glasses��$	��210��$��30��$	��5 0q��� -1   $# tbglassestexture��$��1��#��[ $��30��$��30��� $��2q�y��� $( tbears��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -2   $# tbears��$��0��#��[ $
! Ears��$	��210��$��30��$	��5 0q��� -3   $# tbearstexture��$��1��#��[ $��30��$��30��� $��2q�y��� $( 	tbwatches��$) 
Visibility� �� $
��auto ��� $
��auto �.+��� -4   $# 	tbwatches��$��0��#��[ $! Watches��$	��210��$��30��$	��5 0q��� -5   $# tbwatchestexture��$��1��#��[ $��30��$��30��y��� $( 	tbwatches��$) 
Visibility��� -6   $  ddbracelets��$y�	Collapsed=�$��2q�$��30��$	��250��$! 	Bracelets��$
 	Bracelets��#��[ �� -7   $  ddears��$y�	Collapsed=�$��2q�$��30��$	��250��$
! Ears��$

 Ears��#��[ �� -8   $  	ddglasses��$y�	Collapsed=�$��2q�$��30��$	��250��$! Glasses��$
 Glasses��#��[ �� -9   $  ddhats��$y�	Collapsed=�$��2q�$��30��$	��250��$
! Hats��$

 Hats��#��[ �� -:   $  	ddwatches��$y�	Collapsed=�$��2q�$��30��$	��250��$! Watches��$
 Watches��#��[ � -;   $$ ScreenMessage��  BackgroundBrush#���  $y�	Collapsed=�$��0��$��2�� �� $
��0.2* ��� $
��0.6* ��� $
��0.2* � �� $
��0.2* ��� $
��0.6* ��� $
��0.2* �* � Triggers* :� + :� RoutedEvent$+ Grid.Loaded��.[��� .���� .��W� , W� From$, 0X�	- W� To$- 5X�. g� Duration$. 00:00:03D�/ �� 
TargetName$/ 
blurEffect��0 �� TargetProperty$0 Radius�.+��� 1 J� Opacity$	1 0.1Q�$��3��$��3�� -  'System.Windows.Media.Effects.BlurEffect  2   Radius$2 5Q�-<   
blurEffect����!� 3 j� Stretch$
3 None=�4 !� Visual4 �� $
( Main��� -=   $$ ScreenMessageContainer��$��Center=�$��Center=�$��1��$��1��.+��� $��Select an Outfit��
�������$��22(�$��Center=�$��Center=��� ->   $��PageSGTA��$��startgta��.���� $��Please Start GTA��
���  ��$��22(�$��Center=�$��Center=�