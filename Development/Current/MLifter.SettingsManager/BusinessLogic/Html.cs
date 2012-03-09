using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MLifterSettingsManager.BusinessLogic
{
    class HtmlHelper
    {
        private static readonly string[][] htmlNamedEntities = new string[][] { 
            new string[] { "&quot;", "\"" },
            new string[] { "&lt;", "<" },
            new string[] { "&gt;", ">" },
            new string[] { "&nbsp;", " " },
            new string[] { "&iexcl;", "¡" },
            new string[] { "&cent;", "¢" },
            new string[] { "&pound;", "£" },
            new string[] { "&curren;", "¤" },
            new string[] { "&yen;", "¥" },
            new string[] { "&brvbar;", "¦" },
            new string[] { "&sect;", "§" },
            new string[] { "&uml;", "¨" },
            new string[] { "&copy;", "©" },
            new string[] { "&ordf;", "ª" },
            new string[] { "&laquo;", "«" },
            new string[] { "&not;", "¬" },
            new string[] { "&shy;", "­" },
            new string[] { "&reg;", "®" },
            new string[] { "&macr;", "¯" },
            new string[] { "&deg;", "°" },
            new string[] { "&plusmn;", "±" },
            new string[] { "&sup2;", "²" },
            new string[] { "&sup3;", "³" },
            new string[] { "&acute;", "´" },
            new string[] { "&micro;", "µ" },
            new string[] { "&para;", "¶" },
            new string[] { "&middot;", "·" },
            new string[] { "&cedil;", "¸" },
            new string[] { "&sup1;", "¹" },
            new string[] { "&ordm;", "º" },
            new string[] { "&raquo;", " »" },
            new string[] { "&frac14;", "¼" },
            new string[] { "&frac12;", "½" },
            new string[] { "&frac34;", "¾" },
            new string[] { "&iquest;", "¿" },
            new string[] { "&Agrave;", "À" },
            new string[] { "&Aacute;", "Á" },
            new string[] { "&Acirc;", "Â" },
            new string[] { "&Atilde;", "Ã" },
            new string[] { "&Auml;", "Ä" },
            new string[] { "&Aring;", "Å" },
            new string[] { "&AElig;", "Æ" },
            new string[] { "&Ccedil;", "Ç" },
            new string[] { "&Egrave;", "È" },
            new string[] { "&Eacute;", "É" },
            new string[] { "&Ecirc;", "Ê" },
            new string[] { "&Euml;", "Ë" },
            new string[] { "&Igrave;", "Ì" },
            new string[] { "&Iacute;", "Í" },
            new string[] { "&Icirc;", "Î" },
            new string[] { "&Iuml;", "Ï" },
            new string[] { "&ETH;", "Ð" },
            new string[] { "&Ntilde;", "Ñ" },
            new string[] { "&Ograve;", "Ò" },
            new string[] { "&Oacute;", "Ó" },
            new string[] { "&Ocirc;", "Ô" },
            new string[] { "&Otilde;", "Õ" },
            new string[] { "&Ouml;", "Ö" },
            new string[] { "&times;", "×" },
            new string[] { "&Oslash;", "Ø" },
            new string[] { "&Ugrave;", "Ù" },
            new string[] { "&Uacute;", "Ú" },
            new string[] { "&Ucirc;", "Û" },
            new string[] { "&Uuml;", "Ü" },
            new string[] { "&Yacute;", "Ý" },
            new string[] { "&THORN;", "Þ" },
            new string[] { "&szlig;", "ß" },
            new string[] { "&agrave;", "à" },
            new string[] { "&aacute;", "á" },
            new string[] { "&acirc;", "â" },
            new string[] { "&atilde;", "ã" },
            new string[] { "&auml;", "ä" },
            new string[] { "&aring;", "å" },
            new string[] { "&aelig;", "æ" },
            new string[] { "&ccedil;", "ç" },
            new string[] { "&egrave;", "è" },
            new string[] { "&eacute;", "é" },
            new string[] { "&ecirc;", "ê" },
            new string[] { "&euml;", "ë" },
            new string[] { "&igrave;", "ì" },
            new string[] { "&iacute;", "í" },
            new string[] { "&icirc;", "î" },
            new string[] { "&iuml;", "ï" },
            new string[] { "&eth;", "ð" },
            new string[] { "&ntilde;", "ñ" },
            new string[] { "&ograve;", "ò" },
            new string[] { "&oacute;", "ó" },
            new string[] { "&ocirc;", "ô" },
            new string[] { "&otilde;", "õ" },
            new string[] { "&ouml;", "ö" },
            new string[] { "&divide;", "÷" },
            new string[] { "&oslash;", "ø" },
            new string[] { "&ugrave;", "ù" },
            new string[] { "&uacute;", "ú" },
            new string[] { "&ucirc;", "û" },
            new string[] { "&uuml;", "ü" },
            new string[] { "&yacute;", "ý" },
            new string[] { "&thorn;", "þ" },
            new string[] { "&yuml;", "ÿ" },
            new string[] { "&OElig;", "Œ" },
            new string[] { "&oelig;", "œ" },
            new string[] { "&Scaron;", "Š" },
            new string[] { "&scaron;", "š" },
            new string[] { "&Yuml;", "Ÿ" },
            new string[] { "&fnof;", "ƒ" },
            new string[] { "&circ;", "ˆ" },
            new string[] { "&tilde;", "˜" },
            new string[] { "&Alpha;", "Α" },
            new string[] { "&Beta;", "Β" },
            new string[] { "&Gamma;", "Γ" },
            new string[] { "&Delta;", "Δ" },
            new string[] { "&Epsilon;", "Ε" },
            new string[] { "&Zeta;", "Ζ" },
            new string[] { "&Eta;", "Η" },
            new string[] { "&Theta;", "Θ" },
            new string[] { "&Iota;", "Ι" },
            new string[] { "&Kappa;", "Κ" },
            new string[] { "&Lambda;", "Λ" },
            new string[] { "&Mu;", "Μ" },
            new string[] { "&Nu;", "Ν" },
            new string[] { "&Xi;", "Ξ" },
            new string[] { "&Omicron;", "Ο" },
            new string[] { "&Pi;", "Π" },
            new string[] { "&Rho;", "Ρ" },
            new string[] { "&Sigma;", "Σ" },
            new string[] { "&Tau;", "Τ" },
            new string[] { "&Upsilon;", "Υ" },
            new string[] { "&Phi;", "Φ" },
            new string[] { "&Chi;", "Χ" },
            new string[] { "&Psi;", "Ψ" },
            new string[] { "&Omega;", "Ω" },
            new string[] { "&alpha;", "α" },
            new string[] { "&beta;", "β" },
            new string[] { "&gamma;", "γ" },
            new string[] { "&delta;", "δ" },
            new string[] { "&epsilon;", "ε" },
            new string[] { "&zeta;", "ζ" },
            new string[] { "&eta;", "η" },
            new string[] { "&theta;", "θ" },
            new string[] { "&iota;", "ι" },
            new string[] { "&kappa;", "κ" },
            new string[] { "&lambda;", "λ" },
            new string[] { "&mu;", "μ" },
            new string[] { "&nu;", "ν" },
            new string[] { "&xi;", "ξ" },
            new string[] { "&omicron;", "ο" },
            new string[] { "&pi;", "π" },
            new string[] { "&rho;", "ρ" },
            new string[] { "&sigmaf;", "ς" },
            new string[] { "&sigma;", "σ" },
            new string[] { "&tau;", "τ" },
            new string[] { "&upsilon;", "υ" },
            new string[] { "&phi;", "φ" },
            new string[] { "&chi;", "χ" },
            new string[] { "&psi;", "ψ" },
            new string[] { "&omega;", "ω" },
            new string[] { "&thetasym;", "ϑ" },
            new string[] { "&upsih;", "ϒ" },
            new string[] { "&piv;", "ϖ" },
            new string[] { "&ensp;", " " },
            new string[] { "&emsp;", " " },
            new string[] { "&thinsp;", " " },
            new string[] { "&zwnj;", "‌" },
            new string[] { "&zwj;", "‍" },
            new string[] { "&lrm;", "‎" },
            new string[] { "&rlm;", "‏" },
            new string[] { "&ndash;", "–" },
            new string[] { "&mdash;", "—" },
            new string[] { "&lsquo;", "‘" },
            new string[] { "&rsquo;", "’" },
            new string[] { "&sbquo;", "‚" },
            new string[] { "&ldquo;", "“" },
            new string[] { "&rdquo;", "”" },
            new string[] { "&bdquo;", "„" },
            new string[] { "&dagger;", "†" },
            new string[] { "&Dagger;", "‡" },
            new string[] { "&bull;", "•" },
            new string[] { "&hellip;", "…" },
            new string[] { "&permil;", "‰" },
            new string[] { "&prime;", "′" },
            new string[] { "&Prime;", "″" },
            new string[] { "&lsaquo;", "‹" },
            new string[] { "&rsaquo;", "›" },
            new string[] { "&oline;", "‾" },
            new string[] { "&frasl;", "⁄" },
            new string[] { "&euro;", "€" },
            new string[] { "&image;", "ℑ" },
            new string[] { "&weierp;", "℘" },
            new string[] { "&real;", "ℜ" },
            new string[] { "&trade;", "™" },
            new string[] { "&alefsym;", "ℵ" },
            new string[] { "&larr;", "←" },
            new string[] { "&uarr;", "↑" },
            new string[] { "&rarr;", "→" },
            new string[] { "&darr;", "↓" },
            new string[] { "&harr;", "↔" },
            new string[] { "&crarr;", "↵" },
            new string[] { "&lArr;", "⇐" },
            new string[] { "&uArr;", "⇑" },
            new string[] { "&rArr;", "⇒" },
            new string[] { "&dArr;", "⇓" },
            new string[] { "&hArr;", "⇔" },
            new string[] { "&forall;", "∀" },
            new string[] { "&part;", "∂" },
            new string[] { "&exist;", "∃" },
            new string[] { "&empty;", "∅" },
            new string[] { "&nabla;", "∇" },
            new string[] { "&isin;", "∈" },
            new string[] { "&notin;", "∉" },
            new string[] { "&ni;", "∋" },
            new string[] { "&prod;", "∏" },
            new string[] { "&sum;", "∑" },
            new string[] { "&minus;", "−" },
            new string[] { "&lowast;", "∗" },
            new string[] { "&radic;", "√" },
            new string[] { "&prop;", "∝" },
            new string[] { "&infin;", "∞" },
            new string[] { "&ang;", "∠" },
            new string[] { "&and;", "∧" },
            new string[] { "&or;", "∨" },
            new string[] { "&cap;", "∩" },
            new string[] { "&cup;", "∪" },
            new string[] { "&int;", "∫" },
            new string[] { "&there4;", "∴" },
            new string[] { "&sim;", "∼" },
            new string[] { "&cong;", "≅" },
            new string[] { "&asymp;", "≈" },
            new string[] { "&ne;", "≠" },
            new string[] { "&equiv;", "≡" },
            new string[] { "&le;", "≤" },
            new string[] { "&ge;", "≥" },
            new string[] { "&sub;", "⊂" },
            new string[] { "&sup;", "⊃" },
            new string[] { "&nsub;", "⊄" },
            new string[] { "&sube;", "⊆" },
            new string[] { "&supe;", "⊇" },
            new string[] { "&oplus;", "⊕" },
            new string[] { "&otimes;", "⊗" },
            new string[] { "&perp;", "⊥" },
            new string[] { "&sdot;", "⋅" },
            new string[] { "&lceil;", "⌈" },
            new string[] { "&rceil;", "⌉" },
            new string[] { "&lfloor;", "⌊" },
            new string[] { "&rfloor;", "⌋" },
            new string[] { "&lang;", "〈" },
            new string[] { "&rang;", "〉" },
            new string[] { "&loz;", "◊" },
            new string[] { "&spades;", "♠" },
            new string[] { "&clubs;", "♣" },
            new string[] { "&hearts;", "♥" },
            new string[] { "&diams;", "♦" },
            new string[] { "&amp;", "&" }
        };

        public static string HtmlStripTags(string htmlContent, bool replaceNamedEntities, bool replaceNumberedEntities)
        {
            if (htmlContent == null)
                return null;
            htmlContent = htmlContent.Trim();
            if (htmlContent == string.Empty)
                return string.Empty;

            int bodyStartTagIdx = htmlContent.IndexOf("<body", StringComparison.CurrentCultureIgnoreCase);
            int bodyEndTagIdx = htmlContent.IndexOf("</body>", StringComparison.CurrentCultureIgnoreCase);

            int startIdx = 0, endIdx = htmlContent.Length - 1;
            if (bodyStartTagIdx >= 0)
                startIdx = bodyStartTagIdx;
            if (bodyEndTagIdx >= 0)
                endIdx = bodyEndTagIdx;

            bool insideTag = false,
                    insideAttributeValue = false,
                    insideHtmlComment = false,
                    insideScriptBlock = false,
                    insideNoScriptBlock = false,
                    insideStyleBlock = false;
            char attributeValueDelimiter = '"';

            StringBuilder sb = new StringBuilder(htmlContent.Length);
            for (int i = startIdx; i <= endIdx; i++)
            {

                // html comment block
                if (!insideHtmlComment)
                {
                    if (i + 3 < htmlContent.Length &&
                        htmlContent[i] == '<' &&
                        htmlContent[i + 1] == '!' &&
                        htmlContent[i + 2] == '-' &&
                        htmlContent[i + 3] == '-')
                    {
                        i += 3;
                        insideHtmlComment = true;
                        continue;
                    }
                }
                else // inside html comment
                {
                    if (i + 2 < htmlContent.Length &&
                        htmlContent[i] == '-' &&
                        htmlContent[i + 1] == '-' &&
                        htmlContent[i + 2] == '>')
                    {
                        i += 2;
                        insideHtmlComment = false;
                        continue;
                    }
                    else
                        continue;
                }

                // noscript block
                if (!insideNoScriptBlock)
                {
                    if (i + 9 < htmlContent.Length &&
                        htmlContent[i] == '<' &&
                        (htmlContent[i + 1] == 'n' || htmlContent[i + 1] == 'N') &&
                        (htmlContent[i + 2] == 'o' || htmlContent[i + 2] == 'O') &&
                        (htmlContent[i + 3] == 's' || htmlContent[i + 3] == 'S') &&
                        (htmlContent[i + 4] == 'c' || htmlContent[i + 4] == 'C') &&
                        (htmlContent[i + 5] == 'r' || htmlContent[i + 5] == 'R') &&
                        (htmlContent[i + 6] == 'i' || htmlContent[i + 6] == 'I') &&
                        (htmlContent[i + 7] == 'p' || htmlContent[i + 7] == 'P') &&
                        (htmlContent[i + 8] == 't' || htmlContent[i + 8] == 'T') &&
                        (char.IsWhiteSpace(htmlContent[i + 9]) || htmlContent[i + 9] == '>'))
                    {
                        i += 9;
                        insideNoScriptBlock = true;
                        continue;
                    }
                }
                else // inside noscript block
                {
                    if (i + 10 < htmlContent.Length &&
                        htmlContent[i] == '<' &&
                        htmlContent[i + 1] == '/' &&
                        (htmlContent[i + 2] == 'n' || htmlContent[i + 2] == 'N') &&
                        (htmlContent[i + 3] == 'o' || htmlContent[i + 3] == 'O') &&
                        (htmlContent[i + 4] == 's' || htmlContent[i + 4] == 'S') &&
                        (htmlContent[i + 5] == 'c' || htmlContent[i + 5] == 'C') &&
                        (htmlContent[i + 6] == 'r' || htmlContent[i + 6] == 'R') &&
                        (htmlContent[i + 7] == 'i' || htmlContent[i + 7] == 'I') &&
                        (htmlContent[i + 8] == 'p' || htmlContent[i + 8] == 'P') &&
                        (htmlContent[i + 9] == 't' || htmlContent[i + 9] == 'T') &&
                        (char.IsWhiteSpace(htmlContent[i + 10]) || htmlContent[i + 10] == '>'))
                    {
                        if (htmlContent[i + 10] != '>')
                        {
                            i += 9;
                            while (i < htmlContent.Length && htmlContent[i] != '>')
                                i++;
                        }
                        else
                            i += 10;
                        insideNoScriptBlock = false;
                    }
                    continue;
                }

                // script block
                if (!insideScriptBlock)
                {
                    if (i + 7 < htmlContent.Length &&
                        htmlContent[i] == '<' &&
                        (htmlContent[i + 1] == 's' || htmlContent[i + 1] == 'S') &&
                        (htmlContent[i + 2] == 'c' || htmlContent[i + 2] == 'C') &&
                        (htmlContent[i + 3] == 'r' || htmlContent[i + 3] == 'R') &&
                        (htmlContent[i + 4] == 'i' || htmlContent[i + 4] == 'I') &&
                        (htmlContent[i + 5] == 'p' || htmlContent[i + 5] == 'P') &&
                        (htmlContent[i + 6] == 't' || htmlContent[i + 6] == 'T') &&
                        (char.IsWhiteSpace(htmlContent[i + 7]) || htmlContent[i + 7] == '>'))
                    {
                        i += 6;
                        insideScriptBlock = true;
                        continue;
                    }
                }
                else // inside script block
                {
                    if (i + 8 < htmlContent.Length &&
                        htmlContent[i] == '<' &&
                        htmlContent[i + 1] == '/' &&
                        (htmlContent[i + 2] == 's' || htmlContent[i + 2] == 'S') &&
                        (htmlContent[i + 3] == 'c' || htmlContent[i + 3] == 'C') &&
                        (htmlContent[i + 4] == 'r' || htmlContent[i + 4] == 'R') &&
                        (htmlContent[i + 5] == 'i' || htmlContent[i + 5] == 'I') &&
                        (htmlContent[i + 6] == 'p' || htmlContent[i + 6] == 'P') &&
                        (htmlContent[i + 7] == 't' || htmlContent[i + 7] == 'T') &&
                        (char.IsWhiteSpace(htmlContent[i + 8]) || htmlContent[i + 8] == '>'))
                    {
                        if (htmlContent[i + 8] != '>')
                        {
                            i += 7;
                            while (i < htmlContent.Length && htmlContent[i] != '>')
                                i++;
                        }
                        else
                            i += 8;
                        insideScriptBlock = false;
                    }
                    continue;
                }

                // style block
                if (!insideStyleBlock)
                {
                    if (i + 7 < htmlContent.Length &&
                        htmlContent[i] == '<' &&
                        (htmlContent[i + 1] == 's' || htmlContent[i + 1] == 'S') &&
                        (htmlContent[i + 2] == 't' || htmlContent[i + 2] == 'T') &&
                        (htmlContent[i + 3] == 'y' || htmlContent[i + 3] == 'Y') &&
                        (htmlContent[i + 4] == 'l' || htmlContent[i + 4] == 'L') &&
                        (htmlContent[i + 5] == 'e' || htmlContent[i + 5] == 'E') &&
                        (char.IsWhiteSpace(htmlContent[i + 6]) || htmlContent[i + 6] == '>'))
                    {
                        i += 5;
                        insideStyleBlock = true;
                        continue;
                    }
                }
                else // inside script block
                {
                    if (i + 8 < htmlContent.Length &&
                        htmlContent[i] == '<' &&
                        htmlContent[i + 1] == '/' &&
                        (htmlContent[i + 2] == 's' || htmlContent[i + 2] == 'S') &&
                        (htmlContent[i + 3] == 't' || htmlContent[i + 3] == 'C') &&
                        (htmlContent[i + 4] == 'y' || htmlContent[i + 4] == 'R') &&
                        (htmlContent[i + 5] == 'l' || htmlContent[i + 5] == 'I') &&
                        (htmlContent[i + 6] == 'e' || htmlContent[i + 6] == 'P') &&
                        (char.IsWhiteSpace(htmlContent[i + 7]) || htmlContent[i + 7] == '>'))
                    {
                        if (htmlContent[i + 7] != '>')
                        {
                            i += 7;
                            while (i < htmlContent.Length && htmlContent[i] != '>')
                                i++;
                        }
                        else
                            i += 7;
                        insideStyleBlock = false;
                    }
                    continue;
                }

                if (!insideTag)
                {
                    if (i < htmlContent.Length &&
                        htmlContent[i] == '<')
                    {
                        insideTag = true;
                        continue;
                    }
                }
                else // inside tag
                {
                    if (!insideAttributeValue)
                    {
                        if (htmlContent[i] == '"' || htmlContent[i] == '\'')
                        {
                            attributeValueDelimiter = htmlContent[i];
                            insideAttributeValue = true;
                            continue;
                        }
                        if (htmlContent[i] == '>')
                        {
                            insideTag = false;
                            sb.Append(' '); // prevent words from different tags (<td>s for example) from joining together
                            continue;
                        }
                    }
                    else // inside tag and inside attribute value
                    {
                        if (htmlContent[i] == attributeValueDelimiter)
                        {
                            insideAttributeValue = false;
                            continue;
                        }
                    }
                    continue;
                }

                sb.Append(htmlContent[i]);
            }

            if (replaceNamedEntities)
                foreach (string[] htmlNamedEntity in htmlNamedEntities)
                    sb.Replace(htmlNamedEntity[0], htmlNamedEntity[1]);

            if (replaceNumberedEntities)
                for (int i = 0; i < 512; i++)
                    sb.Replace("&#" + i + ";", ((char)i).ToString());

            return sb.ToString();
        }
    }
}
