RD /S /q ..\setup_files
mkdir ..\setup_files

robocopy .\ ..\setup_files\ *.* /S /E /XD .svn /XF *.svn *.vshost.* *.rar *.zip *.iss *.ini *.bat

set path="C:\Program Files\WinRAR"