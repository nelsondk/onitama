   M S B A M L   `   `   `  ����H  DOnitamaEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:OnitamaEngineOnitamaEngine  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	       OnitamaEngine.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:OnitamaEngine     5   	   9System&clr-namespace:System;assembly=mscorlib     5   	     � Title$  
MainWindow��5	   	   $��555.544��6   $��493.851��6-   .��5
      � $��	0,0,-41,9q�6   .+�5   
   �� -   5         ��Name$ statusLabel��5      $�� ��6%   $��Center=�60   $��26��6M   $��
23,5,161,0q�6Y   $	��Top=�6m   $	��297��6�   $
��Bold"�6�   �� 5   
   -   5         ��Name$ cardListBox��5      $��Center=�6'   $	��337��6D   $��
35,36,87,0q�6Q   $	��Top=�6e   $	��405��6}   $��
10,11,10,5q�6�    �� SelectionMode$ Multiple=�6�   $��Disabled=�6�   $��Disabled=�6%   J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5      5      �� 5   
   -   5        $ redCardListBox��5      $
��Left=�6*   $��82��6E   $��
23,415,0,0q�6Q   $	��Top=�6e   $	��119��6}   ���	#FFE021216�   �� 5   
   $��	Red Cards��6   $
��Left=�6$   $��30��6?   $��
23,385,0,0q�6K   $	��Top=�6_   $	��119��6w   $��Center=�6�   ���	#FFE021216�   �� 5   
   -   5        $ blueCardListBox��5      $
��Left=�6+   $��82��6F   $��165,415,0,0q�6R   $	��Top=�6g   $	��119��6   ���	#FF148ADC6�   �� 5   
   $��
Blue Cards��6   $
��Left=�6%   $��30��6@   $��165,385,0,0q�6L   $	��Top=�6a   $	��119��6y   $��Center=�6�   ���	#FF148ADC6�   �� 5   
   -   5        $ passCardListBox��5      $
��Left=�6+   $��36��6F   $��309,415,0,0q�6R   $	��Top=�6g   $	��119��6   �� 5   
   $��	Pass Card��6   $
��Left=�6$   $��30��6?   $��309,385,0,0q�6K   $	��Top=�6`   $	��119��6x   $��Center=�6�   ���	#FF85898B6�   �� 5   
   -   5         ��Name$ resetButton��5      $��Reset��6&   $
��Left=�66   $��18��6Q   $��309,479,0,0q�6]   $	��Top=�6r   $	��119��6�   $��Center=�6�   $��1,-2,1,1q�6�    �� 	ClickMode$ Press=�6�   ���	#FF7070706�   �� 5   
   -   5        $ startButton��5      $��
Start Game��6&   $
��Left=�6;   $��18��6V   $��309,456,0,0q�6b   $	��Top=�6w   $	��119��6�   $��1,-2,1,1q�6�   {�.  6�   �� 5   
   -   5        $ submitButton��5      $��Submit��6'   $
��Left=�68   $��
353,11,0,0q�6S   $	��Top=�6g   $��75��6   {�.  6�   5      5      