   M S B A M L   `   `   `  ����   Secure file manager"    UserRegistration.AdminWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Admin Window��$	��420��$
��1010�� � 	Resources    CustomButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType �� CornerRadius �� Triggers P� Property	 P� Value  CustomButtonStyle2  	TextStyle  CustomTextBoxStyle6
 ��Name ��Name �� 
TargetName)  #System.Windows.Controls.DataGridRow=  7System.Windows.Controls.Primitives.DataGridColumnHeader&   System.Windows.Controls.DataGrid*  $System.Windows.Controls.DataGridCell  DataGridStyle��%#	  &	        &	 �    &	 V    &	 :    '  �    '      '�� �    '  �    '  P    &	 �    �� " ��.���  � �� �#61CAE6��  � ��
 �������  � ��$ Berlin Sans FB)���  � ��$ 15(���  � ��$ 10q���  � ��$ 2q���  � �� �#61CAE6��  � �� �� " ��.R��� #��z��$ 10��#��z��#��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver	 . . ���  � ��
 �������  � �� �#09AFDB��  � ��$ 2q���  � �� �#09AFDB�� " ��.���  � �� �#61CAE6��  � ��
 �������  � ��$ Berlin Sans FB)���  � ��$ 15(���  � ��$ 10q���  � ��$ 2q���  � �� �#61CAE6��  � �� �� " ��.R��� #��z��$ 10��#��z��#��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver	 . . ���  � ��
 �������  � ��
 � E����  � ��$ 2q���  � ��
 � E���� " ��.���  � ��
 �������  � �� �#61CAE6��  � ��$ Berlin Sans FB)���  � ��$ 20(���  � ��$ 10q���  � �� �� " ��.R��� #��z��$ 15��._��� $��Center=�$��Center=��� " ��.���  � ��
 ���� ��  � ��$ Berlin Sans FB)���  � ��$ 14(���  � ��$ 5q���  � �� �� " ��.R��� $
 border��#��z��
������ $��1q�$ 5��._��� $ PART_ContentHost�� P�  �@J�IsMouseOver	 . . ��� $ border�� � ��
 �����P�  �@J�	IsFocused	 . . ��� $ border�� � ��
 ������� "  .���  � ��$ Berlin Sans FB)���  � ��
 �������  � ��
 �   ��� "  .���  � �� �#61CAE6��  � ��
 �������  � ��$
 Bold"���  � ��$ Arial)���  � ��$ Center=��� " ��.���  � ��$ 2q���  � �� �#61CAE6��  �@��CornerRadius$ 0���� "  .���  � ��
 �������  � ��$ 2q���  � �� �#61CAE6�� "  .���  � ��$ Berlin Sans FB)���  � ��
 �   ��� "  .���  � ��$ Berlin Sans FB)���  � �� �#61CAE6��  � ��
 �������  � ��$ 2q���  � �� �#61CAE6��  �@ AutoGenerateColumns . ��  �@ RowHeaderWidth$ 0Q���  � �� �� "  .R��� #��z��#��z��#��z��._��� .���� .���� .+��� $	��980��$��30�����#61CAE6 �� RadiusX$ 0�� �� RadiusY$ 0�����#61CAE6$��1��$
��Left=�$��Center=� �� Left$ 30���� $.��(E:\C#\Secure file manager\ICONS\logo.png��$��49��$��30��$
��Left=�$	��Top=�$ -1���� $	��256��$	��257�����	#44B5E9F6$	 727��
 �� Top$ 51��$ 30��$ 30�����#61CAE6$��1��$
��Left=�$��Center=�  -     Name$ userDataGrid��   AutoGenerateColumns .  $	��261��$	��684��$
��Left=�$	��Top=�$ 19��$ 47��   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$
 Name��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Name+  $ 	Last Name�� �� *
LastName+  $ Email�� �� *Email+4  .System.Windows.Controls.DataGridCheckBoxColumn  $ IsAdmin�� �� *	IsAdmin+�� $��ACTIONS��#��[ $	 794��$ 65��$	��128��$��30��$
��Left=�$��Center=��� -   $��Delete User��#��[  $	 748��$	 146��$
��Left=�$��Center=�$��85���� -   $��Unblock User��#��[  $	��120��$
��Left=�$	 748��$	 221��$��Center=� J� RenderTransformOrigin$ 0.544,0.593&��� -   $��Add Admin User��#��[  $	��117��$	 748��$	 113��$
��Left=�$��Center=��� -   $��Personal page��#��[  $	��110��$	 803��$ 5��$
��Left=�$��Center=��� -   $��Logout��#��[ $	 923��$ 5��$
��Left=�$��Center=�$��68����  �� Orientation$ Vertical=�$	 748��$	 178��$
��Left=�$��Center=�.+��� $ 
Horizontal=�.+��� -   $��
Block User��#��[  $��22��$��81��(	  "System.Windows.Controls.DatePicker	  -    	 Name$ dpBlockDate��$��31��$��5,0,0,0q��� -	    ��Name$ txtBlockTime��#��[ $��92��$��5,0,0,0q�$��31���� 	 �� X1$ 10��	 �� Y1$ 10��	 �� X2$	 210��	 �� Y2$ 10��
�������$��2��$	 858��$	 196��$
��Left=�$��13��$��Center=�$	��110��