frontend_path=./Oocw.Frontend
server_path=./Oocw.Backend
debug_path=bin/debug/net6.0

cd $frontend_path
npm run build
cd ..

target_path="${server_path}"

if [ -d "${target_path}/wwwroot" ]; then
  echo "${target_path}/wwwroot does exist."
else
  mkdir ${target_path}/wwwroot
fi

cp -r ${frontend_path}/dist/* ${target_path}/wwwroot
dotnet run --project ${server_path}/Oocw.Backend.csproj --configuration Release
