cd ChatSample\Server
call  build.cmd
cd..
cd..
copy ChatSample\Server\Bin\Regulus.Project.Chat.Common.dll assets\project\plugins

copy RegulusUnityKitLibrary\RegulusUnityKitLibrary\Regulus.Remoting.Unity\bin\Debug\protobuf-net.dll assets\regulus\plugins
copy RegulusUnityKitLibrary\RegulusUnityKitLibrary\Regulus.Remoting.Unity\bin\Debug\RegulusRemoting.dll assets\regulus\plugins
copy RegulusUnityKitLibrary\RegulusUnityKitLibrary\Regulus.Remoting.Unity\bin\Debug\RegulusLibrary.dll assets\regulus\plugins
copy RegulusUnityKitLibrary\RegulusUnityKitLibrary\Regulus.Remoting.Unity\bin\Debug\RegulusRemotingGhostNative.dll assets\regulus\plugins
copy RegulusUnityKitLibrary\RegulusUnityKitLibrary\Regulus.Remoting.Unity\bin\Debug\Regulus.Protocol.dll assets\regulus\plugins
copy RegulusUnityKitLibrary\RegulusUnityKitLibrary\Regulus.Remoting.Unity\bin\Debug\Regulus.Remoting.Unity.dll assets\regulus\plugins



pause
