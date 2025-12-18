rm -rf ~/.dotnet
rm -rf ~/.nuget

grep -n "dotnet" ~/.zshrc
grep -n "dotnet" ~/.bashrc


If PATH exports like:

export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$HOME/.dotnet


→ delete 


mkdir -p /goinfre/$USER/dotnet


wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh



For .NET 8:

./dotnet-install.sh --version latest --install-dir /goinfre/$USER/dotnet

versions available:
7.0.404
8.0.100
9.0.306
10.0.100


export DOTNET_ROOT=/goinfre/$USER/dotnet
export PATH=$DOTNET_ROOT:$PATH


source ~/.zshrc


Check:

dotnet --version