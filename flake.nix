{
    description = "Silly bot that tracks when you last said nix";

    inputs = {
        nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
        flake-utils.url = "github:numtide/flake-utils";
    };

    outputs = { self, nixpkgs, flake-utils }:
        let
            packages = flake-utils.lib.eachDefaultSystem (system:
                let
                    pkgs = nixpkgs.legacyPackages."${system}";

                    inputs = with pkgs; [ dotnet-sdk_7 ];
                in rec {
                    packages.nixbot = pkgs.buildDotnetModule {
                        pname = "nixbot";
                        version = "1.1.0";

                        nugetDeps = ./deps.nix;

                        src = ./.;

                        projectFile = "NixBot/NixBot.csproj";

                        meta = with pkgs.lib; {
                            homepage = "https://github.com/lostkagamine/nixbot";
                            description = "silly bot that tracks when you last said nix";
                            license = licenses.mit;
                        };

                        dotnet-sdk = pkgs.dotnet-sdk_7;
                        dotnet-runtime = pkgs.dotnet-runtime_7;
                    };

                    apps.nixbot = flake-utils.lib.mkApp {
                        name = "nixbot";
                        drv = packages.nixbot;
                    };
                } 
            );
        in packages // {
            nixosModule = { config, lib, pkgs, ... }:
                with lib;
                let
                    cfg = config.services.nixbot;
                    pkg = self.packages.${pkgs.system}.nixbot;
                in {
                    options.services.nixbot = {
                        enable = mkEnableOption "nixbot";

                        token = mkOption {
                            type = types.str;
                            default = "";
                        };

                        prefix = mkOption {
                            type = types.str;
                            default = "-";
                        };
                    };

                    config = mkIf cfg.enable {
                        systemd.services.nixbot = {
                            description = "Silly Discord bot";
                            wantedBy = ["multi-user.target"];
                            after = ["network.target"];

                            script = ''

                            if [ ! -d /var/lib/nixbot ]; then
                                mkdir /var/lib/nixbot
                            fi

                            export NIXBOT_DB_PATH="/var/lib/nixbot/nixbot.db"
                            export NIXBOT_TOKEN="${cfg.token}"
                            export NIXBOT_PREFIX="${cfg.prefix}"

                            ${pkg}/bin/NixBot

                            '';
                        };
                    };
                };
        };
}