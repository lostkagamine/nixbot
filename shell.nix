{ pkgs ? import <nixpkgs> {} }:
let
    nixbot = import ./flake.nix;
in
pkgs.mkShell {
  packages = [ nixbot.nixosModule.packages.${pkgs.system}.nixbot ];
}