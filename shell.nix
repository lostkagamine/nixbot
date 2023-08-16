{ pkgs ? import <nixpkgs> {} }:
let
    nixbot = import ./flake.nix;
in
pkgs.mkShell {
  packages = [ nixbot.outputs.packages.${pkgs.system}.nixbot ];
}