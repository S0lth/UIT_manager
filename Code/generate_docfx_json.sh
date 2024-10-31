#!/bin/bash

cat << EOF > docfx.json
{
  "metadata": [
    {
      "src": [
        {
          "files": ["${PROJECT_NAME}/**/*.csproj"],
          "cwd": "${CI_PROJECT_DIR}/Code"
        }
      ],
      "dest": "obj/api"
    }
  ],
  "build": {
    "content": [
      {
        "files": ["api/**/*.yml", "toc.yml"]
      }
    ],
    "dest": "${DOC_OUTPUT}",
    "globalMetadata": {
      "_enableSearch": "true"
    }
  }
}
EOF
