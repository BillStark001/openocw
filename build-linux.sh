frontend_path=./Oocw.Frontend/
server_path=./Oocw.Backend/

cd $frontend_path
npm run build
cd ..

mkdir ${server_path}/wwwroot
cp -r ${frontend_path}/dist/* ${server_path}/wwwroot
dotnet run --project ${server_path}/Oocw.Backend.csproj --configuration Release
