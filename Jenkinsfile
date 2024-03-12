pipeline {
  agent {label 'web-host'}
  stages {
    stage('Checkout') {
      steps {
        checkout scm
      }
    }
    
    stage('Build and upload') {
      steps {
        script {
          def app = docker.build("sambobbarnes/resume-api")
          def build_version = input (id: 'Build_version', message: 'Build version:', parameters: [string(description: 'Build Version Tag', name: 'BUILD_VERSION', trim: true)])
          docker.withRegistry('https://registry.hub.docker.com', 'docker_hub') {
            app.push(build_version)
            app.push("latest")
          }
        }
      }
    }
    
    stage('Deploy') {
      steps {
        writeFile file: '.env', text: "AWS_ACCESS_KEY_ID=${AWS_ACCESS_KEY_ID}\nAWS_SECRET_ACCESS_KEY=${AWS_SECRET_ACCESS_KEY}"
        sh 'echo "$(cat .env)"'
        sh 'docker compose up -d --force-recreate'
      }
    }
  }
}

