frontend_path=./Oocw.Frontend
server_path=./Oocw.Backend
debug_path=bin/debug/net6.0

cd $frontend_path
npm run build
cd ..

if [ -d "${server_path}/${debug_path}" ]; then
  echo "${server_path}/${debug_path} does exist."
else
  mkdir ${server_path}/${debug_path}/wwwroot
fi

cp -r ${frontend_path}/dist/* ${server_path}/${debug_path}/wwwroot
# dotnet run --project ${server_path}/Oocw.Backend.csproj --configuration Release
