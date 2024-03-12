pipeline {
  agent none
  stages {
    stage('Checkout') {
      agent { label 'docker' }
      steps {
        checkout scm
      }
    }
    
    stage('Generate Swagger') {
      agent { label 'docker' }
      steps {
        sh 'cd ./ResumeAPI/ResumeAPI && \
            	dotnet tool restore && \
            	dotnet build --configuration Release -o out && \
            	cd out && \
            	dotnet swagger tofile --output ../../Schemas/swagger.json ResumeAPI.dll v1 && \
            	cd .. && \
            	rm -rf out'
      }
    }
  
    stage('Build and upload') {
      agent { label 'docker' }
      steps {
        input message: 'Build version:', parameters: [string(description: 'Build Version Tag', name: 'BUILD_VERSION', trim: true)]
        sh 'docker build -t sambobbarnes/resume-api:${BUILD_VERSION} .'
        sh 'docker build -t sambobbarnes/resume-api:latest .'
        
        sh 'docker push -a sambobbarnes/resume-api:${BUILD_VERSION}'
        sh 'docker push -a sambobbarnes/resume-api:latest'
      }
    }
    
    stage('Deploy') {
      agent { label 'web-host' }
      steps {
        sh 'docker-compose up -d --force-recreate'
      }
    }
  }
}