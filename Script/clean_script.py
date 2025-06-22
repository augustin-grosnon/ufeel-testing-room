#!/usr/bin/env python3

import os
import shutil

folderName = "Build"

project_root = os.path.abspath("./")
build_dir = os.path.join(project_root, folderName)

def clean_and_create_build():
    if os.path.exists(build_dir):
        shutil.rmtree(build_dir)
    os.makedirs(build_dir)
    print(f"[OK] Dossier '{folderName}/' recréé.")

import os
import shutil

def copy_directory(src, dest):
    if not os.path.exists(src):
        raise FileNotFoundError(f"Source directory does not exist: {src}")

    dest_dir = os.path.join(build_dir, dest)

    if os.path.exists(dest_dir):
        shutil.rmtree(dest_dir)

    shutil.copytree(src, dest_dir)

def copy_files(folder, files):
    for file_path in files:
        abs_src = os.path.join(project_root, file_path)
        rel_dir = os.path.dirname(file_path)
        dest_dir = os.path.join(build_dir, folder, rel_dir)
        os.makedirs(dest_dir, exist_ok=True)
        shutil.copy2(abs_src, dest_dir)

def copy_python_files():
    python_files = [
        "PythonServers/client_base.py",
        "PythonServers/main.py",
        "PythonServers/video_processor.py",
        "PythonServers/eye_tracker.py",
        "PythonServers/emotion_detector.py",
    ]
    copy_files("", python_files)
    copy_directory("PythonServers/models", "PythonServers/models")
    print("[OK] Fichiers Python copiés avec hiérarchie respectée.")


def copy_cs_files():
    cs_files = [
        "UfeelAPI.cs",
        "Server/ClientBase.cs",
        "Server/PythonServerController.cs",
        "EmotionDetection/EmotionReceiver.cs",
        "EyeTracking/EyeTrackingReceiver.cs"
    ]
    copy_files("com.ufcorp.ufeel/Scripts", cs_files)
    print("[OK] Fichiers C# copiés avec hiérarchie respectée.")

def copy_documentation_files():
    documentation_files = [
        "API.md",
        "package.json",
        "UFeel.asmdef"
    ]
    copy_files("", documentation_files)
    print("[OK] Fichiers Documentation copiés avec hiérarchie respectée.")


if __name__ == "__main__":
    clean_and_create_build()
    copy_python_files()
    copy_cs_files()
    copy_documentation_files()
    print(f"[✔] Build complet dans le dossier /{folderName}")
