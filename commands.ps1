& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" install -r "C:\Users\erik_\source\repos\MauiApp6\BeveragesApp\bin\Debug\net10.0-android\com.erikhartlerab.beveragesapp-Signed.apk"

& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" devices

& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" -s "RZCXA135QMW" install -r "C:\Users\erik_\source\repos\MauiApp6\BeveragesApp\bin\Debug\net10.0-android\com.erikhartlerab.beveragesapp-Signed.apk"

& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" -s "RZCXA135QMW" shell getprop ro.product.cpu.abilist

cd C:\Users\erik_\source\repos\MauiApp6\BeveragesApp
dotnet publish BeveragesApp\BeveragesApp.csproj -f net10.0-android -c Debug -p:AndroidPackageFormat=apk -p:RuntimeIdentifier=android-arm64

& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" -s "RZCXA135QMW" logcat -c
& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" -s "RZCXA135QMW" logcat -d -s AndroidRuntime:E

& "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe" -s "RZCXA135QMW" logcat -d -s AndroidRuntime:E



