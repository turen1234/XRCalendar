   M S B A M L   `   `   `  ����V  RExtinction Rebellion, Version=1.0.7341.32485, Culture=neutral, PublicKeyToken=null;"clr-namespace:Extinction_RebellionExtinction_Rebellion  5   	   '    !Extinction_Rebellion.DonationForm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4local"clr-namespace:Extinction_Rebellion     5   	     � Title$  DonationForm��5   	   $��456.004��6   $��450.152��6/   .��5	      � $��20,70,20,20q�6    � ColumnDefinitions 5
      �� 5      $
��134* �6   6%   �� 5      $
��269* �6   6%    � RowDefinitions 5      �� 5      $��40 �6   6!   �� 5      $��40 �6   6!   �� 5      $��40 �6   6!   �� 5      $��40 �6   6!   �� 5      $��40 �6   6!   �� 5      $��40 �6   6!   �� 5      $��* �6   6    .+�5      �� .��6
   Name on Card6   �� 5      -   5        $��FullName��5      $��5q�6    �� TextWrapping$
 Wrap=�6'   $��1��6;   �� 5      $��1��6
   .��6   Number6   �� 5      -   5        $��
CreditCard��5       �� 	MaxLength$ 16��6   $��1��6-   $��5q�6;   $
 Wrap=�6F   $��1��6Z   �� 5      $��2��6
   .��6   CVC6   �� 5       -   5        $	��CVC��5       $��2��6   $
��Left=�6$   $��5q�6@   $��40��6K   $
 Wrap=�6V   $��1��6k   �� 5"      $��3��6
   .��6   Expiry6   �� 5#      $��3��6   $��1��6   $��5q�6-    �� Orientation$ 
Horizontal=�68   .+�5%      �� -   5        $��Expiry_M��5%      $ 2��6   $��30��6+   4  .System.Windows.Automation.AutomationProperties   HelpText$ Month��66   $��MM��6\   �� 5&      -   5        $��Expiry_Y��5&      $ 2��6   $��10,0,0,0q�6,   $��30��6?   $��YY��6J   $
 Year��6T   5(      �� 5)      $��4��6   $��27��6   $��5q�6'   $��2��63   �� 5,      $��5��6
   .��6   Donation Amount6(   �� 5-      -   5        $��Amount��5-      $��5��6   $��5,5.333,157.333,5q�6(   $
 Wrap=�6C   $��1��6W   �� 5.      -   5        $��Checkout_Button��5.      $��Donate��6"   $��1��63   $��35��6C   $��6��6O   $��Bottom=�6\   $��Right=�6w   $	��116��6�   $��10q�6�   6�   �� 5/      $��Bottom=�6
   $
��Left=�6%   $	��114��6@   $��10q�6L   $��powered_by_stripe@2x.png��6X   $��6��6z   $��35��6�   �� 50      $
��Left=�6   $��	0,-51,0,0q�6)   $
��Wrap=�6<   $	��Top=�6P   $��2��6h   $��20��6|   $	��393��6�   �� 6�   $�Please consider ��6�   �� 6�   $�a donation ��6�   �� 6�   $�to support ��6�   �� 6�   $	�Tim��6�   �� 6�   $!�, your rebel app developer!��6�   6   54      55      