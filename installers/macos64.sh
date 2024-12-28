#!/bin/bash

INSTALL_DIR="/usr/local/bin/chronky"
SCRIPT_DIR=$(dirname "$(readlink -f "$0")")

# Ensure the installation directory exists
sudo mkdir -p "$INSTALL_DIR"

# Copy all files to the installation directory
echo "Copying application files to $INSTALL_DIR..."
sudo cp -r "$SCRIPT_DIR/"* "$INSTALL_DIR"

# Ensure the directory is in PATH
if ! echo "$PATH" | grep -q "$INSTALL_DIR"; then
  echo "export PATH=\$PATH:$INSTALL_DIR" | sudo tee /etc/paths.d/chronky > /dev/null
  echo "Added $INSTALL_DIR to PATH. Restart your shell for the changes to take effect."
fi

echo "Installation completed successfully!"
