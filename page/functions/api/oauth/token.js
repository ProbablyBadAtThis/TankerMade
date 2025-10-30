export async function onRequestPost(context) {
    const { request, env } = context;

    try {
        const { code } = await request.json();

        if (!code) {
            return new Response(JSON.stringify({ error: 'Missing authorization code' }), {
                status: 400,
                headers: { 'Content-Type': 'application/json' }
            });
        }

        console.log('Exchanging OAuth code for token...');

        // Exchange code for access token with GitHub
        const tokenResponse = await fetch('https://github.com/login/oauth/access_token', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'User-Agent': 'TankerMade-OAuth/1.0'
            },
            body: JSON.stringify({
                client_id: env.GITHUB_CLIENT_ID,
                client_secret: env.GITHUB_CLIENT_SECRET,
                code: code
            })
        });

        console.log('GitHub response status:', tokenResponse.status);

        // Handle non-JSON responses from GitHub
        const responseText = await tokenResponse.text();
        let tokenData;

        try {
            tokenData = JSON.parse(responseText);
        } catch (e) {
            console.error('GitHub returned non-JSON response:', responseText);
            return new Response(JSON.stringify({ 
                error: 'GitHub OAuth error',
                details: 'Invalid response from GitHub OAuth service'
            }), {
                status: 500,
                headers: { 'Content-Type': 'application/json' }
            });
        }

        if (tokenData.error) {
            console.error('GitHub OAuth error:', tokenData);
            return new Response(JSON.stringify({ 
                error: 'GitHub OAuth error', 
                details: tokenData.error_description || tokenData.error
            }), {
                status: 400,
                headers: { 'Content-Type': 'application/json' }
            });
        }

        if (!tokenData.access_token) {
            console.error('No access token in response:', tokenData);
            return new Response(JSON.stringify({ 
                error: 'OAuth error',
                details: 'No access token received from GitHub'
            }), {
                status: 500,
                headers: { 'Content-Type': 'application/json' }
            });
        }

        console.log('OAuth exchange successful');

        // Return only the access token
        return new Response(JSON.stringify({ 
            access_token: tokenData.access_token 
        }), {
            status: 200,
            headers: { 
                'Content-Type': 'application/json',
                'Cache-Control': 'no-store'
            }
        });

    } catch (error) {
        console.error('OAuth function error:', error);

        return new Response(JSON.stringify({ 
            error: 'Internal server error',
            message: error.message || 'Failed to process OAuth token exchange'
        }), {
            status: 500,
            headers: { 'Content-Type': 'application/json' }
        });
    }
}