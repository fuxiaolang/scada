   M S B A M L   `   `   `  ����W  SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   MyScrollViewer  �� 
TargetType �Name � ColumnDefinitions � RowDefinitions ��Name ��Name �� CanContentScroll �� CanHorizontallyScroll �� CanVerticallyScroll	 ��Name4    .System.Windows.Automation.AutomationProperties
    AutomationId � Cursor �� ScrollableHeight � Maximum � Minimum* �� #ComputedVerticalScrollBarVisibility �� ViewportHeight �� ViewportSize  MyScrollBarStyle � Value �� Mode �� RelativeSource �� ScrollableWidth �� Orientation, �� %ComputedHorizontalScrollBarVisibility �� ViewportWidth  ScrollBarDisabledBackground �� Color  VerticalScrollBarPageButton �� 
TargetType �� Property �� Value  ScrollBarThumb �� RadiusX �� RadiusY �� Triggers  P� Property! P� Value" �� 
TargetName ! HorizontalScrollBarPageButton# J� SnapsToDevicePixels$ ]�Name% J� IsMouseOver& ]� DecreaseRepeatButton' �� PageUpCommand( ]� IncreaseRepeatButton) �� PageDownCommand* ]� Thumb+ �� Triggers, �� PageLeftCommand- �� PageRightCommand�� 5      H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation       :x,http://schemas.microsoft.com/winfx/2006/xaml      5      %�  &	        &	 �    &	     &	 i    &	     &	 g    7  7  7  7  7  7  7  7  �� 5      "  ��6-   .R�5   
   � $
 Grid��6   #��z��6    5      �� 5      $��* �6#   �� 5      $
��Auto �6#    5
      �� 5      $��* �6    �� 5      $
��Auto �6    .+�5      �� $ Corner��6   $��1��6(   
�������$��1��6E   �� 5      $! PART_ScrollContentPresenter��6%   # z��6J   $ False��6�   $ False��6�   #��z��6�   #��z��6�   $��0��6  #��z��6"  $��0��6E  �� 5      $	 PART_VerticalScrollBar��6   $
 VerticalScrollBar��68   $ Arrow��6n   $��1��6}   # z 6�   $ 0Q�6�   $��0��6�   #y�z 6�   # z 6u  #���  6�   6  �� *VerticalOffset+$ OneWay=� �� *TemplatedParent+6�  �� 5      $	 PART_HorizontalScrollBar��6   $
 HorizontalScrollBar��6:   $ Arrow��6r   $��0��6�   # z 6�   $ 0Q�6�   $ 
Horizontal=�6�   $��1��6�   #y�z 6�   # z 6�  #���  6�   64  �� *HorizontalOffset+$ OneWay=� �� *TemplatedParent+6�  5      5      �� 5      $ #F4F4F4��6:   �� 5      " ��60   .�5   
   ��  �@�OverridesDefaultStyle$
 true��62   �� 5   
    � ��
 ���� 6'   �� 5   
    � |�$ false��6&   �� 5   
    � ��$ false��6&   �� 5   
    � �� 5      �� 5      "  ��6"   .R�5       �� #��z��6    #��z��6D   #��z��6f   5!      5#      5$      �� 5&      " n�6#   .�5'   
   ��  �@�OverridesDefaultStyle$
 true��62   �� 5(   
    � ��$ false��6&   �� 5)   
    � �� 5*      �� 5+      "  n�6"   .R�5-      �� $��	thumbRect��6    ���#1C3D58$ 3��6@   $ 3��6L    5/      P� 50        �@J�IsMouseOver$
! True��69   . �51      �� $" 	thumbRect��6E    � �� �#6090BD52      54      56      57      �� 58      " ��62   .�59   
   ��  �@�OverridesDefaultStyle$
 true��62   �� 5:   
    � ��
 ���� 6'   �� 5;   
    � |�$ false��6&   �� 5<   
    � ��$ false��6&   �� 5=   
    � �� 5>      �� 5?      "  ��6"   .R�5@      �� #��z��6    #��z��6D   #��z��6f   5A      5C      5D      �� 5E      " ��6%   .�5F   
   ��  � ��
 �����6'   �� 5G   
    �@��IsPressAndHoldEnabled$ false��69   �� 5H   
    �@��IsFlicksEnabled$ false��63   �� 5J   
    � ��$ 8��6"   �� 5K   
    � ��$ 6��6%   �� 5L   
    � �� 5M      �� 5N      "  ��6"   .R�5P      � $ Bg��6   
������ $
# true��6@   $��8��6[    5Q      �� 5R      .+�5T      ]� $$ 
PART_Track��6    $
��true��64   #{�z% 6O   & 5U      �� 5V   "   #��Z' 6/   8��  6\   ( 5X      �� 5Y   "   #��Z) 6/   8�� 6^   * 5[      n� 5\   "   8�� 6(   5^      5_       5`      P� 5a        � {�$! false��67   . �5b      �� $" Bg��6;   8   � ��6K   5c      5e      5g      + 5h   
   P� 5i        �@��Orientation$! 
Horizontal=�6-   . �5j      ��  � ��$
 Auto��6*   �� 5k       � ��$ 0��6-   �� 5l       � ��$ 6��6+   �� 5m       � ��$ 6��6.   �� 5n       � ��
 �����6/   �� 5o       � �� 5p      �� 5q      "  ��6*   .R�5r      � $ Bg��6#   
������ $
# true��6H    5s   "   �� 5t   &   .+�5v   "   ]� $$ 
PART_Track��6(   #{�z% 6<   & 5w   &   �� 5x   *   #��Z, 67   8�� 6f   ( 5z   &   �� 5{   *   #��Z- 67   8�� 6g   * 5}   &   n� 5~   *   8�� 60   5�   #   5�       5�      P� 5�   "     � {�$! false��6?   . �5�   &   �� $" Bg��6C   8   � ��6S   5�   #   5�      5�      5�      5�      5�      