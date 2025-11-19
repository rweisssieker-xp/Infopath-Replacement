#!/bin/bash
# Deployment script for FormXChange Suite

set -e

ENVIRONMENT=${1:-staging}
NAMESPACE="formxchange"

echo "Deploying FormXChange Suite to ${ENVIRONMENT}..."

# Validate environment
if [[ ! "$ENVIRONMENT" =~ ^(dev|staging|production)$ ]]; then
    echo "Error: Environment must be dev, staging, or production"
    exit 1
fi

# Apply namespace
echo "Creating namespace..."
kubectl apply -f infrastructure/kubernetes/namespaces/formxchange.yaml

# Deploy with Helm
echo "Deploying with Helm..."
helm upgrade --install formxchange-suite ./infrastructure/helm/formxchange-suite \
  --namespace ${NAMESPACE} \
  --values ./infrastructure/helm/formxchange-suite/values.yaml \
  --set environment=${ENVIRONMENT}

echo "Deployment completed!"

