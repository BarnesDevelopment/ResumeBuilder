SHELL=bash

docker-build:
ifdef v
	docker build -t sambobbarnes/resume-api:$(v) .
	docker build -t sambobbarnes/resume-api:latest .
else
	docker build -t sambobbarnes/resume-api .
endif
	
docker-build-push:
ifdef v
	docker build -t sambobbarnes/resume-api:$(v) .
	docker build -t sambobbarnes/resume-api:latest .
	docker push sambobbarnes/resume-api:$(v)
else
	docker build -t sambobbarnes/resume-api:latest .
endif
	docker push sambobbarnes/resume-api:latest
	
docker-test:
	docker build -t sambobbarnes/resume-api:latest .
	docker push sambobbarnes/resume-api
	docker-compose up -d
	
docker-run:
	docker run -p 8080:80/tcp -p 10443:443/tcp sambobbarnes/resume-api
	
docker-build-run:
	docker build -t sambobbarnes/resume-api .
	docker run -p 8080:80/tcp -p 10443:443/tcp -d sambobbarnes/resume-api

generate-swagger:
	cd ./ResumeAPI/ResumeAPI && \
	dotnet tool restore && \
	dotnet build --configuration Release -o out && \
	cd out && \
	dotnet swagger tofile --output ../../Schemas/swagger.json ResumeAPI.dll v1 && \
	cd .. && \
	rm -rf out