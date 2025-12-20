#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

API_URL="http://localhost:5049/api"

echo -e "${YELLOW}=== Testing Authentication API ===${NC}\n"

# Test 1: Create a test employee using dotnet ef
echo -e "${YELLOW}Step 1: Creating test employee...${NC}"
echo "Note: You need to create an employee manually first using:"
echo "dotnet ef dbcontext scaffold or create via code"
echo ""

# Test 2: Login (will fail if no employee exists)
echo -e "${YELLOW}Step 2: Testing Login...${NC}"
LOGIN_RESPONSE=$(curl -s -X POST "$API_URL/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test@1234"
  }')

echo "Login Response:"
echo "$LOGIN_RESPONSE" | jq '.' 2>/dev/null || echo "$LOGIN_RESPONSE"
echo ""

# Extract tokens if login successful
ACCESS_TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.data.accessToken' 2>/dev/null)
REFRESH_TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.data.refreshToken' 2>/dev/null)

if [ "$ACCESS_TOKEN" != "null" ] && [ -n "$ACCESS_TOKEN" ]; then
    echo -e "${GREEN}✓ Login successful!${NC}\n"
    
    # Test 3: Get current user info
    echo -e "${YELLOW}Step 3: Testing Get Current User (with token)...${NC}"
    ME_RESPONSE=$(curl -s -X GET "$API_URL/Auth/me" \
      -H "Authorization: Bearer $ACCESS_TOKEN")
    
    echo "Current User Response:"
    echo "$ME_RESPONSE" | jq '.' 2>/dev/null || echo "$ME_RESPONSE"
    echo ""
    
    # Test 4: Refresh Token
    echo -e "${YELLOW}Step 4: Testing Refresh Token...${NC}"
    REFRESH_RESPONSE=$(curl -s -X POST "$API_URL/Auth/refresh-token" \
      -H "Content-Type: application/json" \
      -d "{
        \"refreshToken\": \"$REFRESH_TOKEN\"
      }")
    
    echo "Refresh Token Response:"
    echo "$REFRESH_RESPONSE" | jq '.' 2>/dev/null || echo "$REFRESH_RESPONSE"
    echo ""
else
    echo -e "${RED}✗ Login failed. Please create a test employee first.${NC}\n"
fi

# Test 5: Forget Password
echo -e "${YELLOW}Step 5: Testing Forget Password...${NC}"
FORGET_RESPONSE=$(curl -s -X POST "$API_URL/Auth/forget-password" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com"
  }')

echo "Forget Password Response:"
echo "$FORGET_RESPONSE" | jq '.' 2>/dev/null || echo "$FORGET_RESPONSE"
echo ""

# Extract reset token if available
RESET_TOKEN=$(echo "$FORGET_RESPONSE" | jq -r '.data.resetToken' 2>/dev/null)

if [ "$RESET_TOKEN" != "null" ] && [ -n "$RESET_TOKEN" ]; then
    # Test 6: Reset Password
    echo -e "${YELLOW}Step 6: Testing Reset Password...${NC}"
    RESET_RESPONSE=$(curl -s -X POST "$API_URL/Auth/reset-password" \
      -H "Content-Type: application/json" \
      -d "{
        \"email\": \"test@example.com\",
        \"token\": \"$RESET_TOKEN\",
        \"newPassword\": \"NewTest@1234\",
        \"confirmPassword\": \"NewTest@1234\"
      }")
    
    echo "Reset Password Response:"
    echo "$RESET_RESPONSE" | jq '.' 2>/dev/null || echo "$RESET_RESPONSE"
    echo ""
fi

echo -e "${YELLOW}=== Testing Complete ===${NC}"

