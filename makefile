SHELL=bash

docker:
	docker compose up -d --force-recreate --pull always

docker-build:
ifdef v
	docker build -f Api.Dockerfile -t ghcr.io/barnesdevelopment/resume-api:$(v) .
	docker build -f Api.Dockerfile -t ghcr.io/barnesdevelopment/resume-api:latest .
else
	docker build -f Api.Dockerfile -t ghcr.io/barnesdevelopment/resume-api .
endif
	
docker-build-push:
ifdef v
	docker build -f Api.Dockerfile -t ghcr.io/barnesdevelopment/resume-api:$(v) .
	docker build -f Api.Dockerfile -t ghcr.io/barnesdevelopment/resume-api:latest .
	docker push ghcr.io/barnesdevelopment/resume-api:$(v)
else
	docker build -f Api.Dockerfile -t ghcr.io/barnesdevelopment/resume-api:latest .
endif
	docker push ghcr.io/barnesdevelopment/resume-api:latest
	
docker-test:
	docker build -f Api.Dockerfile -t ghcr.io/barnesdevelopment/resume-api:test .
	docker-compose up -d
	
docker-run:
	docker run -p 8080:80/tcp -p 10443:443/tcp ghcr.io/barnesdevelopment/resume-api
	
docker-build-run:
	docker build -f Api.Dockerfile -t ghcr.io/barnesdevelopment/resume-api .
	docker run -p 8080:80/tcp -p 10443:443/tcp -d ghcr.io/barnesdevelopment/resume-api

generate-swagger:
	cd ./ResumeAPI/ResumeAPI && \
	dotnet tool restore && \
	dotnet build --configuration Release -o out && \
	cd out && \
	dotnet swagger tofile --output ../../Schemas/swagger.json ResumeAPI.dll v1 && \
	cd .. && \
	rm -rf out
